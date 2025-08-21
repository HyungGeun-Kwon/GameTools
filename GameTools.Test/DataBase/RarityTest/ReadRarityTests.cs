using FluentAssertions;
using GameTools.Application.Features.Rarities.Queries.GetRarityById;
using GameTools.Application.Features.Rarities.Queries.GetRarities;
using GameTools.Infrastructure.Persistence.Stores.ReadStore;
using GameTools.Test.Utils;

namespace GameTools.Test.DataBase.RarityTest
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
            var dto = await handler.Handle(new GetRarityByIdQuery(r.Id), default);

            dto.Should().NotBeNull();
            dto!.Id.Should().Be(r.Id);
            dto.Grade.Should().Be("Common");
            dto.ColorCode.Should().Be("#000000");
            dto.RowVersionBase64.Should().NotBeNullOrEmpty();
            Convert.FromBase64String(dto.RowVersionBase64).Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetRarityById_ReturnsNullWhenNotFound()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var handler = new GetRarityByIdHandler(new RarityReadStore(db));
            var dto = await handler.Handle(new GetRarityByIdQuery(200), default);

            dto.Should().BeNull();
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
            var list = await handler.Handle(new GetRaritiesQuery(), default);

            list.Should().NotBeNull();
            list.Count.Should().BeGreaterThanOrEqualTo(3);
            list.Any(x => x.Id == r1.Id && x.Grade == "Common" && x.ColorCode == "#000000").Should().BeTrue();
            list.Any(x => x.Id == r2.Id && x.Grade == "Rare" && x.ColorCode == "#111111").Should().BeTrue();
            list.Any(x => x.Id == r3.Id && x.Grade == "Epic" && x.ColorCode == "#222222").Should().BeTrue();

            list.All(x => !string.IsNullOrEmpty(x.RowVersionBase64)).Should().BeTrue();
        }

        [Fact]
        public async Task GetRarities_GetEmptyWhenNoRows()
        {
            await using var serverDb = await SqlServerTestDb.CreateAsync();
            var db = serverDb.Db;

            var handler = new GetRaritiesHandler(new RarityReadStore(db));
            var list = await handler.Handle(new GetRaritiesQuery(), default);

            list.Should().NotBeNull();
            list.Should().BeEmpty();
        }
        #endregion
    }
}
