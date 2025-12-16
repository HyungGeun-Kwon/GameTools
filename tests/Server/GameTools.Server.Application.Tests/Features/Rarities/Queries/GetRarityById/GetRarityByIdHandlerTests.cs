using FluentAssertions;
using GameTools.Server.Application.Abstractions.Exceptions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Catalog.Rarities.Models;
using GameTools.Server.Application.Catalog.Rarities.Queries.GetRarityById;
using Moq;
using static GameTools.Server.TestUtilities.Application.Rarities.AppRarityTestData;

namespace GameTools.Server.Application.Tests.Catalog.Rarities.Queries.GetRarityById
{
    public class GetRarityByIdHandlerTests
    {
        private static GetRarityByIdHandler CreateHandler(
            out Mock<IRarityReadStore> rarityReadStoreMock)
        {
            rarityReadStoreMock = new Mock<IRarityReadStore>();
            return new GetRarityByIdHandler(rarityReadStoreMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_Rarity_Does_Not_Exist()
        {
            var handler = CreateHandler(out var rarityReadStoreMock);

            var id = Guid.NewGuid();
            var query = new GetRarityByIdQuery(id);
            var ct = CancellationToken.None;

            rarityReadStoreMock
                .Setup(x => x.GetByIdAsync(id, ct))
                .ReturnsAsync((RarityReadModel?)null);

            var act = async () => await handler.Handle(query, ct);

            await act.Should().ThrowAsync<NotFoundException>();

            rarityReadStoreMock.Verify(x => x.GetByIdAsync(id, ct), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Rarity_When_Exists()
        {
            var handler = CreateHandler(out var rarityReadStoreMock);

            var id = Guid.NewGuid();
            var query = new GetRarityByIdQuery(id);
            var ct = CancellationToken.None;

            var rarity = BuildDefaultRarityReadModel(id);

            rarityReadStoreMock
                .Setup(x => x.GetByIdAsync(id, ct))
                .ReturnsAsync(rarity);

            var result = await handler.Handle(query, ct);

            result.Should().BeSameAs(rarity);
            rarityReadStoreMock.Verify(x => x.GetByIdAsync(id, ct), Times.Once);
        }
    }
}
