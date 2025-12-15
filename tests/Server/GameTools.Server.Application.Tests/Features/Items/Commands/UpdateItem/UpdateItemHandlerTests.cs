using FluentAssertions;
using GameTools.Server.Application.Abstractions.Exceptions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.UnitOfWorks;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using GameTools.Server.Application.Features.Items.Commands.UpdateItem;
using GameTools.Server.Application.Features.Rarities.Models;
using GameTools.Server.Domain.Features.Items.Entities;
using GameTools.Server.Domain.Features.Items.Policies;
using GameTools.Server.Domain.Features.Items.ValueObjects;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using Moq;
using static GameTools.Server.TestUtilities.Domain.Items.DomainItemTestData;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;
using static GameTools.Server.TestUtilities.Application.Rarities.AppRarityTestData;

namespace GameTools.Server.Application.Tests.Features.Items.Commands.UpdateItem
{
    public class UpdateItemHandlerTests
    {
        private static Item BuildItemFromSpec(
            UpdateItemSpec spec,
            string? nameOverride = null,
            Guid? rarityIdOverride = null)
            => new(
                id: ItemId.From(spec.Id),
                name: new ItemName(nameOverride ?? spec.Name),
                price: new ItemPrice(spec.Price),
                description: new ItemDescription(spec.Description),
                rarityId: RarityId.From(rarityIdOverride ?? spec.RarityId));

        private static UpdateItemCommand BuildCommand(UpdateItemSpec? spec = null)
            => new(spec ?? BuildDefaultUpdateItemSpec());

        private static UpdateItemHandler CreateHandler(
            out Mock<IItemWriteStore> itemWriteStoreMock,
            out Mock<IRarityReadStore> rarityReadStoreMock,
            out Mock<IItemNameUniquenessPolicy> namePolicyMock,
            out Mock<IUnitOfWork> uowMock)
        {
            itemWriteStoreMock = new Mock<IItemWriteStore>();
            rarityReadStoreMock = new Mock<IRarityReadStore>();
            namePolicyMock = new Mock<IItemNameUniquenessPolicy>();
            uowMock = new Mock<IUnitOfWork>();

            return new UpdateItemHandler(
                itemWriteStoreMock.Object,
                rarityReadStoreMock.Object,
                namePolicyMock.Object,
                uowMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_Item_Does_Not_Exist()
        {
            var handler = CreateHandler(
                out var itemWriteStoreMock,
                out var rarityReadStoreMock,
                out var namePolicyMock,
                out var uowMock);

            var spec = BuildDefaultUpdateItemSpec();
            var command = BuildCommand(spec);

            // LoadForUpdateAsync 기본값 null → NotFoundException

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();

            itemWriteStoreMock.Verify(x => x.LoadForUpdateAsync(ItemId.From(spec.Id), It.IsAny<CancellationToken>()), Times.Once);
            rarityReadStoreMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            namePolicyMock.Verify(x => x.EnsureUniqueAsync(It.IsAny<ItemName>(), It.IsAny<CancellationToken>()), Times.Never);
            uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_Rarity_Does_Not_Exist()
        {
            var handler = CreateHandler(
                out var itemWriteStoreMock,
                out var rarityReadStoreMock,
                out var namePolicyMock,
                out var uowMock);

            var spec = BuildDefaultUpdateItemSpec();
            var command = BuildCommand(spec);

            var existingItem = BuildItemFromSpec(spec);

            itemWriteStoreMock
                .Setup(x => x.LoadForUpdateAsync(ItemId.From(spec.Id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingItem);

            // Rarity는 null → NotFoundException
            rarityReadStoreMock
                .Setup(x => x.GetByIdAsync(spec.RarityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RarityReadModel?)null);

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();

            itemWriteStoreMock.Verify(x => x.LoadForUpdateAsync(ItemId.From(spec.Id), It.IsAny<CancellationToken>()), Times.Once);
            rarityReadStoreMock.Verify(x => x.GetByIdAsync(spec.RarityId, It.IsAny<CancellationToken>()), Times.Once);
            namePolicyMock.Verify(x => x.EnsureUniqueAsync(It.IsAny<ItemName>(), It.IsAny<CancellationToken>()), Times.Never);
            uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Not_Call_NamePolicy_When_Name_Not_Changed()
        {
            var handler = CreateHandler(
                out var itemWriteStoreMock,
                out var rarityReadStoreMock,
                out var namePolicyMock,
                out var uowMock);

            var spec = BuildDefaultUpdateItemSpec(name: ValidItemNameValue('a'));
            var command = BuildCommand(spec);

            var rarity = BuildDefaultRarityReadModel(spec.RarityId);
            var existingItem = BuildItemFromSpec(spec, nameOverride: spec.Name);

            var rowVersion = ValidItemRowVersion();

            itemWriteStoreMock
                .Setup(x => x.LoadForUpdateAsync(ItemId.From(spec.Id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingItem);

            rarityReadStoreMock
                .Setup(x => x.GetByIdAsync(spec.RarityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rarity);

            itemWriteStoreMock
                .Setup(x => x.GetRowVersion(existingItem))
                .Returns(rowVersion);

            uowMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(existingItem.Id.Value);
            result.RowVersion.Should().BeEquivalentTo(rowVersion);

            namePolicyMock.Verify(
                x => x.EnsureUniqueAsync(It.IsAny<ItemName>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Call_NamePolicy_When_Name_Changed()
        {
            var handler = CreateHandler(
                out var itemWriteStoreMock,
                out var rarityReadStoreMock,
                out var namePolicyMock,
                out var uowMock);

            var originalName = ValidItemNameValue('a');
            var newName = ValidItemNameValue('b');

            var spec = BuildDefaultUpdateItemSpec(name: newName);
            var command = BuildCommand(spec);

            var rarity = BuildDefaultRarityReadModel(spec.RarityId);
            var existingItem = BuildItemFromSpec(spec, nameOverride: originalName);

            var rowVersion = new byte[] { 7, 7, 7, 7 };

            itemWriteStoreMock
                .Setup(x => x.LoadForUpdateAsync(ItemId.From(spec.Id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingItem);

            rarityReadStoreMock
                .Setup(x => x.GetByIdAsync(spec.RarityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rarity);

            namePolicyMock
                .Setup(x => x.EnsureUniqueAsync(
                    It.Is<ItemName>(n => n.Value == newName),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            itemWriteStoreMock
                .Setup(x => x.GetRowVersion(existingItem))
                .Returns(rowVersion);

            uowMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(existingItem.Id.Value);
            result.RowVersion.Should().BeEquivalentTo(rowVersion);

            namePolicyMock.Verify(
                x => x.EnsureUniqueAsync(
                    It.Is<ItemName>(n => n.Value == newName),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}