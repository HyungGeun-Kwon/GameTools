using FluentAssertions;
using GameTools.Server.Application.Abstractions.Exceptions;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.UnitOfWorks;
using GameTools.Server.Application.Features.Rarities.Commands.Common.Specs;
using GameTools.Server.Application.Features.Rarities.Commands.DeleteRarity;
using GameTools.Server.Domain.Features.Rarities.Entities;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using Moq;
using static GameTools.Server.TestUtilities.Application.Rarities.AppRarityTestData;
using static GameTools.Server.TestUtilities.Domain.Rarities.DomainRarityTestData;

namespace GameTools.Server.Application.Tests.Features.Rarities.Commands.CreateRarity
{
    public class CreateRarityHandlerTests
    {
        private static DeleteRarityCommand BuildCommand(DeleteRaritySpec? spec = null)
            => new(spec ?? BuildDefaultDeleteRaritySpec());

        private static DeleteRarityHandler CreateHandler(
            out Mock<IRarityWriteStore> itemWriteStoreMock,
            out Mock<IUnitOfWork> uowMock)
        {
            itemWriteStoreMock = new Mock<IRarityWriteStore>();
            uowMock = new Mock<IUnitOfWork>();

            return new DeleteRarityHandler(
                itemWriteStoreMock.Object,
                uowMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_Rarity_Does_Not_Exist()
        {
            var handler = CreateHandler(
                out var itemWriteStoreMock,
                out var uowMock);

            var spec = BuildDefaultDeleteRaritySpec();
            var command = BuildCommand(spec);

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();

            itemWriteStoreMock.Verify(
                x => x.LoadForUpdateAsync(RarityId.From(spec.Id), It.IsAny<CancellationToken>()),
                Times.Once);

            itemWriteStoreMock.Verify(
                x => x.SetOriginalRowVersion(It.IsAny<Rarity>(), It.IsAny<byte[]>()),
                Times.Never);

            itemWriteStoreMock.Verify(
                x => x.Remove(It.IsAny<Rarity>()),
                Times.Never);

            uowMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Remove_Rarity_When_Exists()
        {
            var handler = CreateHandler(
                out var itemWriteStoreMock,
                out var uowMock);

            var spec = BuildDefaultDeleteRaritySpec();
            var command = BuildCommand(spec);

            var item = BuildRarity(id: ValidRarityId(spec.Id));

            itemWriteStoreMock
                .Setup(x => x.LoadForUpdateAsync(RarityId.From(spec.Id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            uowMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();

            itemWriteStoreMock.Verify(
                x => x.LoadForUpdateAsync(RarityId.From(spec.Id), It.IsAny<CancellationToken>()),
                Times.Once);

            itemWriteStoreMock.Verify(
                x => x.SetOriginalRowVersion(item, spec.RowVersion),
                Times.Once);

            itemWriteStoreMock.Verify(
                x => x.Remove(item),
                Times.Once);

            uowMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}