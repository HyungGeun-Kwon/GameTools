using FluentAssertions;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Bulk;
using GameTools.Server.Application.Catalog.Items.Commands.BulkInsertItems;
using Moq;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;
using GameTools.Server.Application.Abstractions.UnitOfWorks;

namespace GameTools.Server.Application.Tests.Catalog.Items.Commands.BulkInsertItems
{
    public class BulkInsertItemsHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Call_BulkInsertAsync_And_Return_Result()
        {
            var writeStoreMock = new Mock<IItemWriteStore>();
            var uowMock = new Mock<IUnitOfWork>();
            var txMock = new Mock<IUnitOfWorkTransaction>();
            uowMock
                .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(txMock.Object);

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

            txMock
                .Setup(x => x.CommitAsync(ct))
                .Returns(Task.CompletedTask);

            var handler = new BulkInsertItemsHandler(writeStoreMock.Object, uowMock.Object);
            var command = new BulkInsertItemsCommand(specs);

            var result = await handler.Handle(command, ct);

            result.Should().NotBeNull();
            result.BulkResultRows.Should().BeSameAs(bulkResultRows);

            uowMock.Verify(x => x.BeginTransactionAsync(ct), Times.Once);
            writeStoreMock.Verify(x => x.BulkInsertAsync(specs, ct), Times.Once);
            txMock.Verify(x => x.CommitAsync(ct), Times.Once);
            txMock.Verify(x => x.DisposeAsync(), Times.Once);

            writeStoreMock.Verify(x => x.BulkInsertAsync(specs, ct), Times.Once);
            writeStoreMock.VerifyNoOtherCalls();
            txMock.VerifyNoOtherCalls();
        }
    }
}
