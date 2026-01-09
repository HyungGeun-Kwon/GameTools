using System;
using System.Threading.Tasks;
using FluentAssertions;
using GameTools.Server.Domain.Catalog.Rarities.Factories;
using GameTools.Server.Domain.Catalog.Rarities.Policies;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using GameTools.Server.Infrastructure.IntegrationTests.Fixtures;
using GameTools.Server.Infrastructure.Persistence;
using GameTools.Server.Infrastructure.Persistence.Catalog.Checkers;
using GameTools.Server.Infrastructure.Persistence.Catalog.Stores.ReadStore;
using GameTools.Server.Infrastructure.Persistence.Catalog.Stores.WriteStore;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.IntegrationTests.Catalog.Rarities
{
    [Collection(SqlServerDatabaseCollection.Name)]
    public sealed class RarityStoresIntegrationTests : IAsyncLifetime
    {
        private readonly SqlServerDatabaseFixture _dbFixture;
        private readonly TestDbContextFactory _dbFactory;

        public RarityStoresIntegrationTests(SqlServerDatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
            _dbFactory = new TestDbContextFactory(_dbFixture.TestDatabaseConnectionString);
        }

        public async Task InitializeAsync()
        {
            await _dbFixture.EnsureMigratedAsync(() => _dbFactory.Create(actor: "Migrator"));
            await using var db = _dbFactory.Create(actor: "Reset");
            await TestDatabaseReset.ResetAsync(db);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private static RarityFactory CreateFactory_GradeUniqueOnly(AppDbContext db)
        {
            var gradeChecker = new RarityGradeUniquenessChecker(db);
            var gradePolicy = new RarityGradeUniquenessPolicy(gradeChecker);

            return new RarityFactory(gradePolicy);
        }

        [Fact]
        public async Task ReadStore_GetById_Returns_RowVersion()
        {
            await using var db = _dbFactory.Create(actor: "Seeder");
            await TestDatabaseReset.ResetAsync(db);

            var factory = CreateFactory_GradeUniqueOnly(db);
            var writeStore = new RarityWriteStore(db);
            var readStore = new RarityReadStore(db);

            var rarity = await factory.CreateAsync(
                new RarityGrade("Common"),
                new RarityColorCode("#A0A0A0"),
                CancellationToken.None);

            await writeStore.AddAsync(rarity, CancellationToken.None);
            await db.SaveChangesAsync();

            var model = await readStore.GetByIdAsync(rarity.Id.Value, CancellationToken.None);

            model.Should().NotBeNull();
            model!.Id.Should().Be(rarity.Id.Value);
            model.Grade.Should().Be("Common");
            model.ColorCode.Should().Be("#A0A0A0");
            model.RowVersion.Should().NotBeNull();
            model.RowVersion!.Length.Should().Be(8);
        }

        [Fact]
        public async Task WriteStore_GetRowVersion_Returns_8Bytes_After_Save()
        {
            await using var db = _dbFactory.Create(actor: "Seeder");
            await TestDatabaseReset.ResetAsync(db);

            var factory = CreateFactory_GradeUniqueOnly(db);
            var writeStore = new RarityWriteStore(db);

            var rarity = await factory.CreateAsync(
                new RarityGrade("Epic"),
                new RarityColorCode("#A335EE"),
                CancellationToken.None);

            await writeStore.AddAsync(rarity, CancellationToken.None);
            await db.SaveChangesAsync();

            var rv = writeStore.GetRowVersion(rarity);

            rv.Should().NotBeNull();
            rv.Length.Should().Be(8);
        }

        [Fact]
        public async Task WriteStore_SetOriginalRowVersion_Can_Force_Concurrency_Exception()
        {
            Guid id;

            // 1) seed
            await using (var seedDb = _dbFactory.Create(actor: "Seeder"))
            {
                await TestDatabaseReset.ResetAsync(seedDb);

                var factory = CreateFactory_GradeUniqueOnly(seedDb);
                var writeStore = new RarityWriteStore(seedDb);

                var rarity = await factory.CreateAsync(
                    new RarityGrade("Before"),
                    new RarityColorCode("#123456"),
                    CancellationToken.None);

                await writeStore.AddAsync(rarity, CancellationToken.None);
                await seedDb.SaveChangesAsync();

                id = rarity.Id.Value;
            }

            // 2) update with wrong original rowversion
            await using var db = _dbFactory.Create(actor: "User");
            var write = new RarityWriteStore(db);

            var loaded = await write.LoadForUpdateAsync(RarityId.From(id), CancellationToken.None);
            loaded.Should().NotBeNull();

            loaded!.ChangeGrade(new RarityGrade("After"));

            // 존재할 수 없는 rowversion으로 설정 (길이만 맞춰줌)
            write.SetOriginalRowVersion(loaded, new byte[8]);

            Func<Task> act = () => db.SaveChangesAsync();
            await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
        }
    }
}
