using FluentAssertions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Catalog.Rarities.Models;
using GameTools.Server.Application.Catalog.Rarities.Queries.GetAllRarities;
using Moq;
using static GameTools.Server.TestUtilities.Application.Rarities.AppRarityTestData;

namespace GameTools.Server.Application.Tests.Catalog.Rarities.Queries.GetAllRarities
{
    public class GetAllRaritiesHandlerTests
    {
        private static GetAllRaritiesHandler CreateHandler(
            out Mock<IRarityReadStore> rarityReadStoreMock)
        {
            rarityReadStoreMock = new Mock<IRarityReadStore>();
            return new GetAllRaritiesHandler(rarityReadStoreMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_All_Rarities_From_Store()
        {
            var handler = CreateHandler(out var rarityReadStoreMock);

            var query = new GetAllRaritiesQuery();
            var ct = CancellationToken.None;

            var rarities = new List<RarityReadModel>
            {
                BuildDefaultRarityReadModel(),
                BuildDefaultRarityReadModel()
            };

            rarityReadStoreMock
                .Setup(x => x.GetAllAsync(ct))
                .ReturnsAsync(rarities);

            var result = await handler.Handle(query, ct);

            result.Should().BeEquivalentTo(rarities);
            rarityReadStoreMock.Verify(x => x.GetAllAsync(ct), Times.Once);
        }
    }
}
