using FluentAssertions;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Bulk;
using GameTools.Server.Application.Catalog.Items.Commands.BulkInsertItems;
using Moq;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;

namespace GameTools.Server.Application.Tests.Catalog.Items.Commands.BulkInsertItems
{
    public class BulkInsertItemsHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Call_BulkInsertAsync_And_Return_Result()
        {
            var writeStoreMock = new Mock<IItemWriteStore>();
            var handler = new BulkInsertItemsHandler(writeStoreMock.Object);

            var specs = new[]
            {
                BuildDefaultCreateItemSpec(),
                BuildDefaultCreateItemSpec()
            };

            var ct = CancellationToken.None;

            IReadOnlyList<BulkResultRow> bulkResultRows = [];

            writeStoreMock
                .Setup(x => x.BulkInsertAsync(specs, ct))
                .ReturnsAsync(bulkResultRows);

            var command = new BulkInsertItemsCommand(specs);

            var result = await handler.Handle(command, ct);

            result.Should().NotBeNull();
            result.BulkResultRows.Should().BeSameAs(bulkResultRows);

            writeStoreMock.Verify(x => x.BulkInsertAsync(specs, ct), Times.Once);
            writeStoreMock.VerifyNoOtherCalls();
        }
    }
}
