using FluentAssertions;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.UnitOfWorks;
using GameTools.Server.Application.Catalog.Items.Commands.BulkDeleteItems;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Bulk;
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
            var uowMock = new Mock<IUnitOfWork>();
            var txMock = new Mock<IUnitOfWorkTransaction>();
            uowMock
                .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(txMock.Object);

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
            
            txMock
                .Setup(x => x.CommitAsync(ct))
                .Returns(Task.CompletedTask);

            var handler = new BulkDeleteItemsHandler(writeStoreMock.Object, uowMock.Object);
            var command = new BulkDeleteItemsCommand(specs);

            var result = await handler.Handle(command, ct);

            result.Should().NotBeNull();
            result.BulkResultRows.Should().BeSameAs(bulkResultRows);

            uowMock.Verify(x => x.BeginTransactionAsync(ct), Times.Once);
            writeStoreMock.Verify(x => x.BulkDeleteAsync(specs, ct), Times.Once);
            txMock.Verify(x => x.CommitAsync(ct), Times.Once);
            txMock.Verify(x => x.DisposeAsync(), Times.Once);

            writeStoreMock.Verify(x => x.BulkDeleteAsync(specs, ct), Times.Once);
            writeStoreMock.VerifyNoOtherCalls();
            txMock.VerifyNoOtherCalls();
        }
    }
}
