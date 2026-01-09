using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.UnitOfWorks;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;
using GameTools.Server.Application.Catalog.Items.Commands.UpdateItem;
using GameTools.Server.Domain.Catalog.Items.Factories;
using GameTools.Server.Domain.Catalog.Items.Policies;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;
using GameTools.Server.Domain.Catalog.Rarities.Factories;
using GameTools.Server.Domain.Catalog.Rarities.Policies;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using GameTools.Server.Infrastructure.IntegrationTests.Fixtures;
using GameTools.Server.Infrastructure.Persistence;
using GameTools.Server.Infrastructure.Persistence.Catalog.Checkers;
using GameTools.Server.Infrastructure.Persistence.Catalog.Stores.ReadStore;
using GameTools.Server.Infrastructure.Persistence.Catalog.Stores.WriteStore;
using GameTools.Server.Infrastructure.Persistence.Works;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.IntegrationTests.Catalog.Items
{
    [Collection(SqlServerDatabaseCollection.Name)]
    public sealed class UpdateItemHandlerIntegrationTests : IAsyncLifetime
    {
        private readonly SqlServerDatabaseFixture _fx;
        private readonly TestDbContextFactory _dbFactory;

        public UpdateItemHandlerIntegrationTests(SqlServerDatabaseFixture fx)
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
        public async Task UpdateItemHandler_Updates_And_Returns_New_RowVersion()
        {
            await using var db = _dbFactory.Create(actor: "Updater");
            await TestDatabaseReset.ResetAsync(db);

            // seed rarity
            var rarityFactory = CreateRarityFactory(db);
            var rarity = await rarityFactory.CreateAsync(new RarityGrade("Common"), new RarityColorCode("#A0A0A0"), default);
            db.Rarities.Add(rarity);
            await db.SaveChangesAsync();

            // seed item
            var itemFactory = new ItemFactory(new NoopItemNameUniquenessPolicy());
            var item = await itemFactory.CreateAsync(
                new ItemName("Potion"),
                new ItemPrice(10),
                new ItemDescription("HP +50"),
                rarity.Id,
                default);

            db.Items.Add(item);
            await db.SaveChangesAsync();

            // 현재 rowversion 확보
            var writeStore = new ItemWriteStore(db);
            var originalRv = writeStore.GetRowVersion(item); // shadow RowVersion

            // handler 구성 (실제 스토어/유닛오브워크 사용)
            IItemWriteStore itemWrite = writeStore;
            IRarityReadStore rarityRead = new RarityReadStore(db);
            IUnitOfWork uow = new UnitOfWork(db);

            var handler = new UpdateItemHandler(itemWrite, rarityRead, new NoopItemNameUniquenessPolicy(), uow);

            var cmd = new UpdateItemCommand(new UpdateItemSpec(
                Id: item.Id.Value,
                Name: "Potion",              // 이름 동일 -> 유니크 체크 안 탐
                Price: 99,
                Description: "HP +999",
                RarityId: rarity.Id.Value,
                RowVersion: originalRv));

            var result = await handler.Handle(cmd, default);

            result.Id.Should().Be(item.Id.Value);
            result.RowVersion.Should().NotBeNull();
            result.RowVersion!.Length.Should().Be(8);

            // 저장값 확인
            var updated = await db.Items.AsNoTracking().SingleAsync(i => i.Id == item.Id);
            updated.Price.Value.Should().Be(99);
            updated.Description.Value.Should().Be("HP +999");
        }
    }
}
