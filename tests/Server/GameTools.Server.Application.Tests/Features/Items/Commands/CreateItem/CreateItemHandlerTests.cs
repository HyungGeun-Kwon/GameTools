using FluentAssertions;
using GameTools.Server.Application.Abstractions.Exceptions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.UnitOfWorks;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;
using GameTools.Server.Application.Catalog.Items.Commands.CreateItem;
using GameTools.Server.Domain.Catalog.Items.Entities;
using GameTools.Server.Domain.Catalog.Items.Factories;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using Moq;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;
using static GameTools.Server.TestUtilities.Application.Rarities.AppRarityTestData;

namespace GameTools.Server.Application.Tests.Catalog.Items.Commands.CreateItem
{
    public class CreateItemHandlerTests
    {
        private static Item BuildItemFromSpec(CreateItemSpec spec, ItemId? id = null)
            => new (
                id: id ?? ItemId.New(),
                name: new ItemName(spec.Name),
                price: new ItemPrice(spec.Price),
                description: new ItemDescription(spec.Description),
                rarityId: RarityId.From(spec.RarityId));

        private static CreateItemCommand BuildCommand(CreateItemSpec? spec = null)
            => new(spec ?? BuildDefaultCreateItemSpec());

        private static CreateItemHandler CreateHandler(
            out Mock<IItemWriteStore> itemWriteStoreMock,
            out Mock<IRarityReadStore> rarityReadStoreMock,
            out Mock<IItemFactory> itemFactoryMock,
            out Mock<IUnitOfWork> uowMock)
        {
            itemWriteStoreMock = new Mock<IItemWriteStore>();
            rarityReadStoreMock = new Mock<IRarityReadStore>();
            itemFactoryMock = new Mock<IItemFactory>();
            uowMock = new Mock<IUnitOfWork>();

            return new CreateItemHandler(
                itemWriteStoreMock.Object,
                rarityReadStoreMock.Object,
                itemFactoryMock.Object,
                uowMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_Rarity_Does_Not_Exist()
        {
            var handler = CreateHandler(
                out var itemWriteStoreMock,
                out var rarityReadStoreMock,
                out var itemFactoryMock,
                out var uowMock);

            var command = BuildCommand();

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();

            // Rarity조회는 1회 실행되어야함.
            rarityReadStoreMock.Verify(x => x.GetByIdAsync(command.Spec.RarityId, It.IsAny<CancellationToken>()), Times.Once);

            // 나머지는 실행되지 않아아야함.
            itemFactoryMock.Verify(x => x.CreateAsync(
                It.IsAny<ItemName>(),
                It.IsAny<ItemPrice>(),
                It.IsAny<ItemDescription>(),
                It.IsAny<RarityId>(),
                It.IsAny<CancellationToken>()), Times.Never);
            itemWriteStoreMock.Verify(x => x.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Never);
            uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Create_And_Return_When_Rarity_Exists()
        {
            var handler = CreateHandler(
                out var itemWriteStoreMock,
                out var rarityReadStoreMock,
                out var itemFactoryMock,
                out var uowMock);

            var spec = BuildDefaultCreateItemSpec();
            var command = BuildCommand(spec);

            var rarity = BuildDefaultRarityReadModel(id: spec.RarityId);
            
            var item = BuildItemFromSpec(spec);
            var itemRowVersion = new byte[] { 5, 6, 7, 8 };

            itemWriteStoreMock.Setup(x => x.GetRowVersion(item)).Returns(itemRowVersion);
            uowMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            rarityReadStoreMock.Setup(x => x.GetByIdAsync(spec.RarityId, It.IsAny<CancellationToken>())).ReturnsAsync(rarity);
            itemFactoryMock
                .Setup(x => x.CreateAsync(
                    It.Is<ItemName>(n => n.Value == spec.Name),
                    It.Is<ItemPrice>(p => p.Value == spec.Price),
                    It.Is<ItemDescription>(d => d.Value == spec.Description),
                    It.Is<RarityId>(r => r.Value == rarity.Id),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(item.Id.Value);
            result.RowVersion.Should().BeEquivalentTo(itemRowVersion);

            rarityReadStoreMock.Verify(x => x.GetByIdAsync(spec.RarityId, It.IsAny<CancellationToken>()), Times.Once);
            itemFactoryMock.Verify(x => x.CreateAsync(
                It.Is<ItemName>(n => n.Value == spec.Name),
                It.Is<ItemPrice>(p => p.Value == spec.Price),
                It.Is<ItemDescription>(d => d.Value == spec.Description),
                It.Is<RarityId>(r => r.Value == rarity.Id),
                It.IsAny<CancellationToken>()), Times.Once);
            itemWriteStoreMock.Verify(x => x.AddAsync(item, It.IsAny<CancellationToken>()), Times.Once);
            uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            itemWriteStoreMock.Verify(x => x.GetRowVersion(item), Times.Once);
        }
    }
}
