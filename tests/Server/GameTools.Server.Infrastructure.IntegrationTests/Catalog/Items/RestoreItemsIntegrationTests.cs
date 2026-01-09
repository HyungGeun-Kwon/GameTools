using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GameTools.Server.Application.Auditing.Common;
using GameTools.Server.Application.Operations.Restores.Commands.RestoreItems;
using GameTools.Server.Domain.Catalog.Items.Factories;
using GameTools.Server.Domain.Catalog.Items.Policies;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;
using GameTools.Server.Domain.Catalog.Rarities.Factories;
using GameTools.Server.Domain.Catalog.Rarities.Policies;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using GameTools.Server.Infrastructure.IntegrationTests.Fixtures;
using GameTools.Server.Infrastructure.Persistence;
using GameTools.Server.Infrastructure.Persistence.Catalog.Checkers;
using GameTools.Server.Infrastructure.Persistence.Operations.Restores.Stores.ReadStores;
using GameTools.Server.Infrastructure.Persistence.Operations.Restores.Stores.WriteStores;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.IntegrationTests.Catalog.Items
{
    [Collection(SqlServerDatabaseCollection.Name)]
    public sealed class RestoreItemsIntegrationTests : IAsyncLifetime
    {
        private readonly SqlServerDatabaseFixture _fx;
        private readonly TestDbContextFactory _dbFactory;

        public RestoreItemsIntegrationTests(SqlServerDatabaseFixture fx)
        {
            _fx = fx;
            _dbFactory = new TestDbContextFactory(_fx.TestDatabaseConnectionString);
        }

        public async Task InitializeAsync()
        {
            await _fx.EnsureMigratedAsync(() => _dbFactory.Create("Migrator"));
            await using var db = _dbFactory.Create("Reset");
            await TestDatabaseReset.ResetAsync(db);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private static RarityFactory CreateRarityFactory(AppDbContext db)
        {
            var gradeChecker = new RarityGradeUniquenessChecker(db);
            var gradePolicy = new RarityGradeUniquenessPolicy(gradeChecker);
            return new RarityFactory(gradePolicy);
        }

        private sealed class NoopItemNameUniquenessPolicy : IItemNameUniquenessPolicy
        {
            public Task EnsureUniqueAsync(ItemName value, CancellationToken ct) => Task.CompletedTask;
        }

        [Fact]
        public async Task Restore_DryRun_DoesNotChangeItem_But_WritesHistory()
        {
            await using var db = _dbFactory.Create(actor: "Restorer");
            await TestDatabaseReset.ResetAsync(db);

            // rarity + item 생성
            var rarityFactory = CreateRarityFactory(db);
            var rarity = await rarityFactory.CreateAsync(new RarityGrade("Common"), new RarityColorCode("#A0A0A0"), default);
            db.Rarities.Add(rarity);
            await db.SaveChangesAsync();

            var itemFactory = new ItemFactory(new NoopItemNameUniquenessPolicy());
            var item = await itemFactory.CreateAsync(new ItemName("Potion"), new ItemPrice(10), new ItemDescription("v1"), rarity.Id, default);
            db.Items.Add(item);
            await db.SaveChangesAsync();

            // Insert audit 시간 확보 (asOf를 “insert 직후”로 잡기)
            var insertAuditTime = await db.ItemAudits.AsNoTracking()
                .Where(a => a.ItemId == item.Id.Value && a.Action == AuditAction.INSERT)
                .Select(a => a.ChangedAtUtc)
                .OrderByDescending(t => t)
                .FirstAsync();

            var asOf = insertAuditTime.AddMilliseconds(1);

            // update 한 번 발생 (복구 대상)
            item.ChangeDescription(new ItemDescription("v2"));
            await db.SaveChangesAsync();

            var auditCountBefore = await db.ItemAudits.CountAsync();

            // DryRun restore
            var store = new RestoreItemWriteStore(db);
            var res = await store.RestoreItemsAsOfAsync(new RestoreItemsSpec(
                AsOfUtc: asOf,
                ItemIds: [item.Id.Value],
                DryRun: true,
                Notes: "dry"
            ), default);

            res.IsChanged.Should().BeTrue(); // 결과셋 반환

            // DryRun은 롤백이므로 아이템은 그대로(v2)
            var current = await db.Items.AsNoTracking().SingleAsync(i => i.Id == item.Id);
            current.Description.Value.Should().Be("v2");

            // Restore 중 audit_skip=1이므로 감사 로그가 추가로 쌓이면 안 됨
            var auditCountAfter = await db.ItemAudits.CountAsync();
            auditCountAfter.Should().Be(auditCountBefore);

            // RestoreHistory는 기록되어야 함 (ReadStore로 확인)
            var historyStore = new ItemRestoreHistoryReadStore(db);
            var history = await historyStore.GetByIdAsync(res.RestoreId, default);

            history.Should().NotBeNull();
            history!.DryRun.Should().BeTrue();
            history.Actor.Should().Be("Restorer");
        }

        [Fact]
        public async Task Restore_Commit_RevertsItem_To_AsOf()
        {
            await using var db = _dbFactory.Create(actor: "Restorer");
            await TestDatabaseReset.ResetAsync(db);

            var rarityFactory = CreateRarityFactory(db);
            var rarity = await rarityFactory.CreateAsync(new RarityGrade("Common"), new RarityColorCode("#A0A0A0"), default);
            db.Rarities.Add(rarity);
            await db.SaveChangesAsync();

            var itemFactory = new ItemFactory(new NoopItemNameUniquenessPolicy());
            var item = await itemFactory.CreateAsync(new ItemName("Potion"), new ItemPrice(10), new ItemDescription("v1"), rarity.Id, default);
            db.Items.Add(item);
            await db.SaveChangesAsync();

            var insertAuditTime = await db.ItemAudits.AsNoTracking()
                .Where(a => a.ItemId == item.Id.Value && a.Action == AuditAction.INSERT)
                .Select(a => a.ChangedAtUtc)
                .OrderByDescending(t => t)
                .FirstAsync();

            var asOf = insertAuditTime.AddMilliseconds(1);

            item.ChangeDescription(new ItemDescription("v2"));
            await db.SaveChangesAsync();

            // 실제 restore (commit)
            var store = new RestoreItemWriteStore(db);
            var res = await store.RestoreItemsAsOfAsync(new RestoreItemsSpec(
                AsOfUtc: asOf,
                ItemIds: [item.Id.Value],
                DryRun: false,
                Notes: "commit"
            ), default);

            res.IsChanged.Should().BeTrue();

            // v2 -> v1로 되돌아가야 함 (restore proc는 BeforeJson 기반으로 역연산)
            var current = await db.Items.AsNoTracking().SingleAsync(i => i.Id == item.Id);
            current.Description.Value.Should().Be("v1");
        }
    }
}
