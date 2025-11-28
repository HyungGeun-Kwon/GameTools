using FluentAssertions;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Features.Items.Commands.BulkUpdateItems;
using GameTools.Server.Application.Features.Items.Commands.Common.Bulk;
using Moq;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;

namespace GameTools.Server.Application.Tests.Features.Items.Commands.BulkUpdateItems
{
    public class BulkUpdateItemsHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Call_BulkUpdateAsync_And_Return_Result()
        {
            var writeStoreMock = new Mock<IItemWriteStore>();
            var handler = new BulkUpdateItemsHandler(writeStoreMock.Object);

            var specs = new[]
            {
                BuildDefaultUpdateItemSpec(),
                BuildDefaultUpdateItemSpec()
            };

            var ct = CancellationToken.None;
            IReadOnlyList<BulkResultRow> bulkResultRows = [];

            writeStoreMock
                .Setup(x => x.BulkUpdateAsync(specs, ct))
                .ReturnsAsync(bulkResultRows);

            var command = new BulkUpdateItemsCommand(specs);

            var result = await handler.Handle(command, ct);

            result.Should().NotBeNull();
            result.BulkResultRows.Should().BeSameAs(bulkResultRows);

            writeStoreMock.Verify(x => x.BulkUpdateAsync(specs, ct), Times.Once);
            writeStoreMock.VerifyNoOtherCalls();
        }
    }
}
