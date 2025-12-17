using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GameTools.Server.Domain.Catalog.Rarities.Entities;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using GameTools.Server.Infrastructure.IntegrationTests.Fixtures;
using GameTools.Server.Infrastructure.Persistence.Catalog.Stores.WriteStore;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.IntegrationTests.Catalog.Rarities
{
    [Collection(SqlServerDatabaseCollection.Name)]
    public sealed class RarityWriteStoreIntegrationTests(SqlServerDatabaseFixture fx)
    {
        [Fact]
        public async Task Add_And_Save_Should_Persist_And_Generate_RowVersion()
        {
            var factory = new TestDbContextFactory(fx.TestDatabaseConnectionString);
            await fx.EnsureMigratedAsync(() => factory.Create());

            await using var db = factory.Create(actor: "RarityTest");
            var store = new RarityWriteStore(db);

            // Domain Factory를 안 쓰는 순수 통합테스트 버전(최소 경로)
            // 만약 Rarity 생성이 internal이면 기존 Factory를 주입해서 생성해도 됨.
            var rarity = typeof(Rarity)
                .GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                                binder: null,
                                types: new[] { typeof(RarityId), typeof(RarityGrade), typeof(RarityColorCode) },
                                modifiers: null)
                ?.Invoke(new object[]
                {
                    RarityId.New(), new RarityGrade("Mythic"), new RarityColorCode("#ABCDEF")
                }) as Rarity;

            rarity.Should().NotBeNull("Rarity 생성자가 internal이면 테스트에서 factory를 쓰는 게 더 좋음");

            await store.AddAsync(rarity!, CancellationToken.None);
            await db.SaveChangesAsync();

            var rowVersion = store.GetRowVersion(rarity!);
            rowVersion.Should().NotBeNull();
            rowVersion.Length.Should().BeGreaterThan(0);

            var loaded = await db.Rarities.AsNoTracking().SingleAsync(r => r.Grade == new RarityGrade("Mythic"));
            loaded.Should().NotBeNull();
        }
    }
}
