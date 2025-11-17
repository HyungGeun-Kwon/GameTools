using FluentAssertions;
using GameTools.Server.Application.Features.Rarities.Queries.GetRarityById;
using GameTools.Server.Application.Features.Rarities.Queries.GetRarities;
using GameTools.Server.Infrastructure.Persistence.Stores.ReadStore;
using GameTools.Server.Test.Utils;

namespace GameTools.Server.Test.DataBase.RarityTest
{
    public class ReadRarityTests
    {
        #region GetRarityById
        [Fact]
        public async Task GetRarityById()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var r = serverDb.SeedRarity("Common", "#000000");

            var handler = new GetRarityByIdHandler(new RarityReadStore(db));
            var rarityReadModel = await handler.Handle(new GetRarityByIdQuery(r.Id), CancellationToken.None);

            rarityReadModel.Should().NotBeNull();
            rarityReadModel!.Id.Should().Be(r.Id);
            rarityReadModel.Grade.Should().Be("Common");
            rarityReadModel.ColorCode.Should().Be("#000000");
            rarityReadModel.RowVersion.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetRarityById_ReturnsNullWhenNotFound()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var handler = new GetRarityByIdHandler(new RarityReadStore(db));
            var rarityReadModel = await handler.Handle(new GetRarityByIdQuery(200), CancellationToken.None);

            rarityReadModel.Should().BeNull();
        }
        #endregion

        #region GetRarities
        [Fact]
        public async Task GetRarities_GetAllRarities()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var r1 = serverDb.SeedRarity("Common", "#000000");
            var r2 = serverDb.SeedRarity("Rare", "#111111");
            var r3 = serverDb.SeedRarity("Epic", "#222222");

            var handler = new GetRaritiesHandler(new RarityReadStore(db));
            var list = await handler.Handle(new GetRaritiesQuery(), CancellationToken.None);

            list.Should().NotBeNull();
            list.Count.Should().BeGreaterThanOrEqualTo(3);
            list.Any(x => x.Id == r1.Id && x.Grade == "Common" && x.ColorCode == "#000000").Should().BeTrue();
            list.Any(x => x.Id == r2.Id && x.Grade == "Rare" && x.ColorCode == "#111111").Should().BeTrue();
            list.Any(x => x.Id == r3.Id && x.Grade == "Epic" && x.ColorCode == "#222222").Should().BeTrue();

            list.All(x => x.RowVersion?.Length > 0).Should().BeTrue();
        }

        [Fact]
        public async Task GetRarities_GetEmptyWhenNoRows()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var handler = new GetRaritiesHandler(new RarityReadStore(db));
            var list = await handler.Handle(new GetRaritiesQuery(), CancellationToken.None);

            list.Should().NotBeNull();
            list.Should().BeEmpty();
        }
        #endregion
    }
}
