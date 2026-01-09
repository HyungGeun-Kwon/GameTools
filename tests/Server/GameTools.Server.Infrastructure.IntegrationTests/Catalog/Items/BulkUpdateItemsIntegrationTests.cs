using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GameTools.Server.Application.Catalog.Items.Commands.BulkUpdateItems;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;
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
using GameTools.Server.Infrastructure.Persistence.Works;

namespace GameTools.Server.Infrastructure.IntegrationTests.Catalog.Items
{
    [Collection(SqlServerDatabaseCollection.Name)]
    public sealed class BulkUpdateItemsIntegrationTests : IAsyncLifetime
    {
        private readonly SqlServerDatabaseFixture _fx;
        private readonly TestDbContextFactory _dbFactory;

        public BulkUpdateItemsIntegrationTests(SqlServerDatabaseFixture fx)
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
        public async Task BulkUpdate_Returns_Success_And_Concurrency()
        {
            await using var db = _dbFactory.Create(actor: "BulkUser");
            await TestDatabaseReset.ResetAsync(db);

            // rarity
            var rarityFactory = CreateRarityFactory(db);
            var rarity = await rarityFactory.CreateAsync(new RarityGrade("Common"), new RarityColorCode("#A0A0A0"), default);
            db.Rarities.Add(rarity);
            await db.SaveChangesAsync();

            // items seed (EF)
            var itemFactory = new ItemFactory(new NoopItemNameUniquenessPolicy());
            var i1 = await itemFactory.CreateAsync(new ItemName("AAAA"), new ItemPrice(1), new ItemDescription("d1"), rarity.Id, default);
            var i2 = await itemFactory.CreateAsync(new ItemName("BBBB"), new ItemPrice(2), new ItemDescription("d2"), rarity.Id, default);
            db.Items.AddRange(i1, i2);
            await db.SaveChangesAsync();

            var writeStore = new ItemWriteStore(db);

            // i1은 정상 rowversion, i2는 틀린 rowversion(동시성)
            var rv1 = writeStore.GetRowVersion(i1);
            var badRv = new byte[8]; // 존재할 수 없는 원본값

            var specs = new List<UpdateItemSpec>
            {
                new(Id: i1.Id.Value, Name: "AAAA", Price: 10, Description: "u1", RarityId: rarity.Id.Value, RowVersion: rv1),
                new(Id: i2.Id.Value, Name: "BBBB", Price: 20, Description: "u2", RarityId: rarity.Id.Value, RowVersion: badRv),
            };

            var handler = new BulkUpdateItemsHandler(writeStore, new UnitOfWork(db));
            var result = await handler.Handle(new BulkUpdateItemsCommand(specs), default);

            // 성공 row는 RowVersion이 채워져야 함
            var r0 = result.BulkResultRows.Single(x => x.Index == 0);
            r0.RowVersion.Should().NotBeNull();
            r0.ErrorCode.Should().BeNull();

            // 동시성 row는 ErrorCode가 "Concurrency"
            var r1 = result.BulkResultRows.Single(x => x.Index == 1);
            r1.ErrorCode.Should().Be("Concurrency");
        }
    }
}
