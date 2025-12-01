using FluentAssertions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Items.Models;
using GameTools.Server.Application.Features.Items.Queries.GetItemsPage;
using Moq;
using static GameTools.Server.Application.Tests.Features.Items.Queries.GetItemsPage.GetItemsPageTestData;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;

namespace GameTools.Server.Application.Tests.Features.Items.Queries.GetItemsPage
{
    public class GetItemsPageHandlerTests
    {
        private static GetItemsPageHandler CreateHandler(out Mock<IItemReadStore> itemReadStoreMock)
        {
            itemReadStoreMock = new Mock<IItemReadStore>();
            return new GetItemsPageHandler(itemReadStoreMock.Object);
        }
        private static PagedResult<ItemReadModel> CreateItemReadModelPagedResult()
        {
            var readModel = BuildDefaultItemReadModel();
            return new(
                Items: [readModel],
                TotalCount: 1,
                PageNumber: 1,
                PageSize: 10);
        }
        [Fact]
        public async Task Handle_Should_Return_PagedResult_From_Store()
        {
            var handler = CreateHandler(out var itemReadStoreMock);

            var criteria = BuildCriteria();

            var query = new GetItemsPageQuery(criteria);
            var ct = CancellationToken.None;

            var pagedResult = CreateItemReadModelPagedResult();

            itemReadStoreMock
                .Setup(x => x.GetPageAsync(criteria, ct))
                .ReturnsAsync(pagedResult);

            var result = await handler.Handle(query, ct);

            result.Should().BeSameAs(pagedResult);
            itemReadStoreMock.Verify(x => x.GetPageAsync(criteria, ct), Times.Once);
        }
    }
}
