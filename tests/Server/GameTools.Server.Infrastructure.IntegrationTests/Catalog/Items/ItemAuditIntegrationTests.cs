using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GameTools.Server.Application.Auditing.Common;
using GameTools.Server.Domain.Catalog.Items.Factories;
using GameTools.Server.Domain.Catalog.Items.Policies;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;
using GameTools.Server.Domain.Catalog.Rarities.Factories;
using GameTools.Server.Domain.Catalog.Rarities.Policies;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using GameTools.Server.Infrastructure.IntegrationTests.Fixtures;
using GameTools.Server.Infrastructure.Persistence;
using GameTools.Server.Infrastructure.Persistence.Catalog.Checkers;
using GameTools.Server.Infrastructure.Persistence.Catalog.Stores.WriteStore;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.IntegrationTests.Catalog.Items
{
    [Collection(SqlServerDatabaseCollection.Name)]
    public sealed class ItemAuditIntegrationTests : IAsyncLifetime
    {
        private readonly SqlServerDatabaseFixture _fx;
        private readonly TestDbContextFactory _dbFactory;

        public ItemAuditIntegrationTests(SqlServerDatabaseFixture fx)
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

        private static ItemFactory CreateItemFactory_NoUniq()
            => new(new NoopItemNameUniquenessPolicy());

        private sealed class NoopItemNameUniquenessPolicy : IItemNameUniquenessPolicy
        {
            public Task EnsureUniqueAsync(ItemName value, CancellationToken ct) => Task.CompletedTask;
        }

        [Fact]
        public async Task SaveChanges_Inserts_ItemAudit_With_Actor()
        {
            await using var db = _dbFactory.Create(actor: "Alice");
            await TestDatabaseReset.ResetAsync(db);

            // rarity 먼저
            var rarityFactory = CreateRarityFactory(db);
            var rarity = await rarityFactory.CreateAsync(new RarityGrade("Common"), new RarityColorCode("#A0A0A0"), default);
            db.Rarities.Add(rarity);
            await db.SaveChangesAsync();

            // item 생성/저장 (EF)
            var itemFactory = CreateItemFactory_NoUniq();
            var item = await itemFactory.CreateAsync(
                new ItemName("Potion"),
                new ItemPrice(10),
                new ItemDescription("HP +50"),
                rarity.Id,
                default);

            var write = new ItemWriteStore(db);
            await write.AddAsync(item, default);
            await db.SaveChangesAsync(); // actor 세팅 후 저장

            // 트리거가 ItemAudit 생성
            var audit = await db.ItemAudits.AsNoTracking()
                .Where(a => a.ItemId == item.Id.Value && a.Action == AuditAction.INSERT)
                .OrderByDescending(a => a.ChangedAtUtc)
                .FirstOrDefaultAsync();

            audit.Should().NotBeNull();
            audit!.ChangedBy.Should().Be("Alice");
            (audit.AfterJson ?? "").Should().Contain("\"Name\":\"Potion\"");
        }

        [Fact]
        public async Task SaveChanges_Update_Writes_Update_Audit()
        {
            await using var db = _dbFactory.Create(actor: "Bob");
            await TestDatabaseReset.ResetAsync(db);

            var rarityFactory = CreateRarityFactory(db);
            var rarity = await rarityFactory.CreateAsync(new RarityGrade("Common"), new RarityColorCode("#A0A0A0"), default);
            db.Rarities.Add(rarity);
            await db.SaveChangesAsync();

            var itemFactory = CreateItemFactory_NoUniq();
            var item = await itemFactory.CreateAsync(
                new ItemName("Sword"),
                new ItemPrice(100),
                new ItemDescription("Old"),
                rarity.Id,
                default);

            db.Items.Add(item);
            await db.SaveChangesAsync();

            // 수정
            item.ChangePrice(new ItemPrice(200));
            item.ChangeDescription(new ItemDescription("New"));
            await db.SaveChangesAsync();

            var audit = await db.ItemAudits.AsNoTracking()
                .Where(a => a.ItemId == item.Id.Value && a.Action == AuditAction.UPDATE)
                .OrderByDescending(a => a.ChangedAtUtc)
                .FirstOrDefaultAsync();

            audit.Should().NotBeNull();
            audit!.ChangedBy.Should().Be("Bob");
            (audit.AfterJson ?? "").Should().Contain("\"Price\":200");
            (audit.BeforeJson ?? "").Should().Contain("\"Price\":100");
        }
    }
}
