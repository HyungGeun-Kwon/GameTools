using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace GameTools.Server.Application.Tests.Features.Items.Commands.UpdateItem
{
    public class UpdateItemHandlerTests
    {
        private static string ValidItemNameStr(char c = 'a') => new(c, ItemName.MinLength);

        private static string ValidItemDescriptionStr()
            => new('d', Math.Min(10, ItemDescription.MaxLength));

        private static string ValidRarityGradeStr()
            => new('r', RarityGrade.MinLength);

        private static string ValidRarityColorCodeStr()
            => "#FFFFFF";

        private static byte[] ValidRowVersion() => [1, 2, 3, 4];

        private static RarityReadModel BuildValidRarityReadModel(Guid? id = null)
            => new(
                Id: id ?? Guid.NewGuid(),
                Grade: ValidRarityGradeStr(),
                ColorCode: ValidRarityColorCodeStr(),
                RowVersion: ValidRowVersion());

        private static UpdateItemSpec BuildValidSpec(
            Guid? id = null,
            string? name = null,
            int? price = null,
            string? description = null,
            Guid? rarityId = null,
            byte[]? rowVersion = null)
            => new(
                Id: id ?? Guid.NewGuid(),
                Name: name ?? ValidItemNameStr(),
                Price: price ?? ItemPrice.MinValue,
                Description: description ?? ValidItemDescriptionStr(),
                RarityId: rarityId ?? Guid.NewGuid(),
                RowVersion: rowVersion ?? ValidRowVersion());

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
            => new(spec ?? BuildValidSpec());

        private static UpdateItemHandler CreateHandler(
            Mock<IItemWriteStore>? itemWriteStoreMock = null,
            Mock<IRarityReadStore>? rarityReadStoreMock = null,
            Mock<IItemNameUniquenessPolicy>? namePolicyMock = null,
            Mock<IUnitOfWork>? uowMock = null)
        {
            itemWriteStoreMock ??= new Mock<IItemWriteStore>();
            rarityReadStoreMock ??= new Mock<IRarityReadStore>();
            namePolicyMock ??= new Mock<IItemNameUniquenessPolicy>();
            uowMock ??= new Mock<IUnitOfWork>();

            return new UpdateItemHandler(
                itemWriteStoreMock.Object,
                rarityReadStoreMock.Object,
                namePolicyMock.Object,
                uowMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_Item_Does_Not_Exist()
        {
            var itemWriteStoreMock = new Mock<IItemWriteStore>();
            var rarityReadStoreMock = new Mock<IRarityReadStore>();
            var namePolicyMock = new Mock<IItemNameUniquenessPolicy>();
            var uowMock = new Mock<IUnitOfWork>();

            var handler = CreateHandler(
                itemWriteStoreMock,
                rarityReadStoreMock,
                namePolicyMock,
                uowMock);

            var spec = BuildValidSpec();
            var command = BuildCommand(spec);

            // LoadForUpdateAsync 기본값 null → NotFoundException

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Item '{spec.Id}' not found.");

            itemWriteStoreMock.Verify(x => x.LoadForUpdateAsync(spec.Id, It.IsAny<CancellationToken>()), Times.Once);
            rarityReadStoreMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            namePolicyMock.Verify(x => x.EnsureUniqueAsync(It.IsAny<ItemName>(), It.IsAny<CancellationToken>()), Times.Never);
            uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_Rarity_Does_Not_Exist()
        {
            var itemWriteStoreMock = new Mock<IItemWriteStore>();
            var rarityReadStoreMock = new Mock<IRarityReadStore>();
            var namePolicyMock = new Mock<IItemNameUniquenessPolicy>();
            var uowMock = new Mock<IUnitOfWork>();

            var handler = CreateHandler(
                itemWriteStoreMock,
                rarityReadStoreMock,
                namePolicyMock,
                uowMock);

            var spec = BuildValidSpec();
            var command = BuildCommand(spec);

            var existingItem = BuildItemFromSpec(spec);

            itemWriteStoreMock
                .Setup(x => x.LoadForUpdateAsync(spec.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingItem);

            // Rarity는 null → NotFoundException
            rarityReadStoreMock
                .Setup(x => x.GetByIdAsync(spec.RarityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RarityReadModel?)null);

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Rarity '{spec.RarityId}' not found.");

            itemWriteStoreMock.Verify(x => x.LoadForUpdateAsync(spec.Id, It.IsAny<CancellationToken>()), Times.Once);
            rarityReadStoreMock.Verify(x => x.GetByIdAsync(spec.RarityId, It.IsAny<CancellationToken>()), Times.Once);
            namePolicyMock.Verify(x => x.EnsureUniqueAsync(It.IsAny<ItemName>(), It.IsAny<CancellationToken>()), Times.Never);
            uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Not_Call_NamePolicy_When_Name_Not_Changed()
        {
            var itemWriteStoreMock = new Mock<IItemWriteStore>();
            var rarityReadStoreMock = new Mock<IRarityReadStore>();
            var namePolicyMock = new Mock<IItemNameUniquenessPolicy>();
            var uowMock = new Mock<IUnitOfWork>();

            var handler = CreateHandler(
                itemWriteStoreMock,
                rarityReadStoreMock,
                namePolicyMock,
                uowMock);

            var spec = BuildValidSpec(name: ValidItemNameStr('a'));
            var command = BuildCommand(spec);

            var rarity = BuildValidRarityReadModel(spec.RarityId);
            var existingItem = BuildItemFromSpec(spec, nameOverride: spec.Name);

            var rowVersion = new byte[] { 9, 9, 9, 9 };

            itemWriteStoreMock
                .Setup(x => x.LoadForUpdateAsync(spec.Id, It.IsAny<CancellationToken>()))
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
            var itemWriteStoreMock = new Mock<IItemWriteStore>();
            var rarityReadStoreMock = new Mock<IRarityReadStore>();
            var namePolicyMock = new Mock<IItemNameUniquenessPolicy>();
            var uowMock = new Mock<IUnitOfWork>();

            var handler = CreateHandler(
                itemWriteStoreMock,
                rarityReadStoreMock,
                namePolicyMock,
                uowMock);

            var originalName = ValidItemNameStr('a');
            var newName = ValidItemNameStr('b');

            var spec = BuildValidSpec(name: newName);
            var command = BuildCommand(spec);

            var rarity = BuildValidRarityReadModel(spec.RarityId);
            var existingItem = BuildItemFromSpec(spec, nameOverride: originalName);

            var rowVersion = new byte[] { 7, 7, 7, 7 };

            itemWriteStoreMock
                .Setup(x => x.LoadForUpdateAsync(spec.Id, It.IsAny<CancellationToken>()))
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