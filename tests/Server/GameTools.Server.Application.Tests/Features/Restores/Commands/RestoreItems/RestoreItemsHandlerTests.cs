using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Features.Restores.Commands.RestoreItems;
using Moq;
using static GameTools.Server.TestUtilities.Application.Restores.AppRestoreItemTestData;

namespace GameTools.Server.Application.Tests.Features.Restores.Commands.RestoreItems
{
    public class RestoreItemsHandlerTests
    {
        private static RestoreItemsCommand BuildCommand(RestoreItemsSpec? spec = null)
            => new(spec ?? BuildDefaultRestoreItemsSpec());

        private static RestoreItemsHandler CreateHandler(out Mock<IRestoreItemWriteStore> restoreItemWriteStoreMock)
        {
            restoreItemWriteStoreMock = new Mock<IRestoreItemWriteStore>();

            return new RestoreItemsHandler(restoreItemWriteStoreMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Call_WriteStore_And_Return_IsChanged_False_When_No_Changes()
        {
            var handler = CreateHandler(out var writeStoreMock);

            var spec = BuildDefaultRestoreItemsSpec();
            var command = BuildCommand(spec);

            var restoreId = Guid.NewGuid();
            var storeResult = new RestoreItemsStoreResult(
                RestoreId: restoreId,
                Deleted: 0,
                Inserted: 0,
                Updated: 0);

            writeStoreMock
                .Setup(x => x.RestoreItemsAsOfAsync(spec, It.IsAny<CancellationToken>()))
                .ReturnsAsync(storeResult);

            var result = await handler.Handle(command, CancellationToken.None);

            result.RestoreId.Should().Be(restoreId);
            result.Deleted.Should().Be(0);
            result.Inserted.Should().Be(0);
            result.Updated.Should().Be(0);
            result.IsChanged.Should().BeFalse();

            writeStoreMock.Verify(
                x => x.RestoreItemsAsOfAsync(spec, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Call_WriteStore_And_Return_IsChanged_True_When_Any_Changes()
        {
            var handler = CreateHandler(out var writeStoreMock);

            var spec = BuildDefaultRestoreItemsSpec();
            var command = BuildCommand(spec);

            var restoreId = Guid.NewGuid();
            var storeResult = new RestoreItemsStoreResult(
                RestoreId: restoreId,
                Deleted: 1,
                Inserted: 2,
                Updated: 3);

            writeStoreMock
                .Setup(x => x.RestoreItemsAsOfAsync(spec, It.IsAny<CancellationToken>()))
                .ReturnsAsync(storeResult);

            var result = await handler.Handle(command, CancellationToken.None);

            result.RestoreId.Should().Be(restoreId);
            result.Deleted.Should().Be(1);
            result.Inserted.Should().Be(2);
            result.Updated.Should().Be(3);
            result.IsChanged.Should().BeTrue();

            writeStoreMock.Verify(
                x => x.RestoreItemsAsOfAsync(spec, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
