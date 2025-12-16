using FluentAssertions;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Bulk;
using GameTools.Server.Application.Catalog.Items.Commands.BulkDeleteItems;
using Moq;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;

namespace GameTools.Server.Application.Tests.Catalog.Items.Commands.BulkDeleteItems
{
    public class BulkDeleteItemsHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Call_BulkDeleteAsync_And_Return_Result()
        {
            var writeStoreMock = new Mock<IItemWriteStore>();
            var handler = new BulkDeleteItemsHandler(writeStoreMock.Object);

            var specs = new[]
            {
                BuildDefaultDeleteItemSpec(),
                BuildDefaultDeleteItemSpec()
            };

            var ct = CancellationToken.None;
            IReadOnlyList<BulkResultRow> bulkResultRows = [];

            writeStoreMock
                .Setup(x => x.BulkDeleteAsync(specs, ct))
                .ReturnsAsync(bulkResultRows);

            var command = new BulkDeleteItemsCommand(specs);

            var result = await handler.Handle(command, ct);

            result.Should().NotBeNull();
            result.BulkResultRows.Should().BeSameAs(bulkResultRows);

            writeStoreMock.Verify(x => x.BulkDeleteAsync(specs, ct), Times.Once);
            writeStoreMock.VerifyNoOtherCalls();
        }
    }
}
