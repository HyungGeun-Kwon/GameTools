using FluentAssertions;
using GameTools.Server.Application.Abstractions.Exceptions;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.UnitOfWorks;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using GameTools.Server.Application.Features.Items.Commands.DeleteItem;
using Moq;
using static GameTools.Server.TestUtilities.Domain.Items.DomainItemTestData;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;
using GameTools.Server.Domain.Features.Items.Entities;

namespace GameTools.Server.Application.Tests.Features.Items.Commands.DeleteItem
{
    public class DeleteItemHandlerTests
    {
        private static DeleteItemCommand BuildCommand(DeleteItemSpec? spec = null)
            => new(spec ?? BuildDefaultDeleteItemSpec());

        private static DeleteItemHandler CreateHandler(
            Mock<IItemWriteStore>? itemWriteStoreMock = null,
            Mock<IUnitOfWork>? uowMock = null)
        {
            itemWriteStoreMock ??= new Mock<IItemWriteStore>();
            uowMock ??= new Mock<IUnitOfWork>();

            return new DeleteItemHandler(
                itemWriteStoreMock.Object,
                uowMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_Item_Does_Not_Exist()
        {
            var itemWriteStoreMock = new Mock<IItemWriteStore>();
            var uowMock = new Mock<IUnitOfWork>();

            var handler = CreateHandler(itemWriteStoreMock, uowMock);

            var spec = BuildDefaultDeleteItemSpec();
            var command = BuildCommand(spec);

            var act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();

            itemWriteStoreMock.Verify(
                x => x.LoadForUpdateAsync(spec.Id, It.IsAny<CancellationToken>()),
                Times.Once);

            itemWriteStoreMock.Verify(
                x => x.SetOriginalRowVersion(It.IsAny<Item>(), It.IsAny<byte[]>()),
                Times.Never);

            itemWriteStoreMock.Verify(
                x => x.Remove(It.IsAny<Item>()),
                Times.Never);

            uowMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Remove_Item_When_Exists()
        {
            var itemWriteStoreMock = new Mock<IItemWriteStore>();
            var uowMock = new Mock<IUnitOfWork>();

            var handler = CreateHandler(itemWriteStoreMock, uowMock);

            var spec = BuildDefaultDeleteItemSpec();
            var command = BuildCommand(spec);

            var item = BuildItem(id: ValidItemId(spec.Id));

            itemWriteStoreMock
                .Setup(x => x.LoadForUpdateAsync(spec.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            uowMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();

            itemWriteStoreMock.Verify(
                x => x.LoadForUpdateAsync(spec.Id, It.IsAny<CancellationToken>()),
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
