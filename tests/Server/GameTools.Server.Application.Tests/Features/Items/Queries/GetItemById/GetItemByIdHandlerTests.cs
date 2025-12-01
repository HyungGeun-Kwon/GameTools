using FluentAssertions;
using GameTools.Server.Application.Abstractions.Exceptions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Features.Items.Models;
using GameTools.Server.Application.Features.Items.Queries.GetItemById;
using Moq;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;

namespace GameTools.Server.Application.Tests.Features.Items.Queries.GetItemById
{
    public class GetItemByIdHandlerTests
    {
        private static GetItemByIdHandler CreateHandler(out Mock<IItemReadStore> itemReadStoreMock)
        {
            itemReadStoreMock = new Mock<IItemReadStore>();
            return new GetItemByIdHandler(itemReadStoreMock.Object);
        }
        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_Item_Does_Not_Exist()
        {
            var handler = CreateHandler(out var itemReadStoreMock);

            var id = Guid.NewGuid();
            var query = new GetItemByIdQuery(id);
            var ct = CancellationToken.None;

            itemReadStoreMock
                .Setup(x => x.GetByIdAsync(id, ct))
                .ReturnsAsync((ItemReadModel?)null);

            var act = async () => await handler.Handle(query, ct);

            await act.Should().ThrowAsync<NotFoundException>();

            itemReadStoreMock.Verify(x => x.GetByIdAsync(id, ct), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Item_When_Exists()
        {
            var handler = CreateHandler(out var itemReadStoreMock);

            var id = Guid.NewGuid();
            var query = new GetItemByIdQuery(id);
            var ct = CancellationToken.None;

            var item = BuildDefaultItemReadModel();

            itemReadStoreMock
                .Setup(x => x.GetByIdAsync(id, ct))
                .ReturnsAsync(item);

            var result = await handler.Handle(query, ct);

            result.Should().BeSameAs(item);
            itemReadStoreMock.Verify(x => x.GetByIdAsync(id, ct), Times.Once);
        }
    }
}
