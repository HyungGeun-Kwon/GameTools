using FluentAssertions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Auditing.Queries.GetItemAuditPage;
using GameTools.Server.Application.Common.Paging;
using Moq;

namespace GameTools.Server.Application.Tests.Catalog.Audit.Queries.GetItemAuditPage
{
    public class GetItemAuditPageHandlerTests
    {
        private static GetItemAuditPageHandler CreateHandler(
            out Mock<IItemAuditReadStore> auditReadStoreMock)
        {
            auditReadStoreMock = new Mock<IItemAuditReadStore>();
            return new GetItemAuditPageHandler(auditReadStoreMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_PagedResult_From_Store()
        {
            var handler = CreateHandler(out var auditReadStoreMock);

            var criteria = new ItemAuditPageCriteria(
                Pagination: new Pagination(),
                Filter: null,
                SortBy: nameof(ItemAuditReadModel.Id),
                Desc: true);

            var query = new GetItemAuditPageQuery(criteria);
            var ct = CancellationToken.None;

            var items = new[]
            {
                new ItemAuditReadModel(
                    Id: Guid.NewGuid(),
                    ItemId: Guid.NewGuid(),
                    Action: "INSERT",
                    BeforeJson: null,
                    AfterJson: "{}",
                    ChangedAtUtc: DateTime.UtcNow,
                    ChangedBy: "tester"),
                new ItemAuditReadModel(
                    Id: Guid.NewGuid(),
                    ItemId: Guid.NewGuid(),
                    Action: "UPDATE",
                    BeforeJson: "{}",
                    AfterJson: "{\"name\":\"changed\"}",
                    ChangedAtUtc: DateTime.UtcNow,
                    ChangedBy: "tester2"),
            };

            var pagedResult = new PagedResult<ItemAuditReadModel>(
                Items: items,
                PageNumber: 1,
                PageSize: 10,
                TotalCount: items.Length);

            auditReadStoreMock
                .Setup(x => x.GetItemAuditPageAsync(criteria, ct))
                .ReturnsAsync(pagedResult);

            var result = await handler.Handle(query, ct);

            result.Should().BeSameAs(pagedResult);
            auditReadStoreMock.Verify(x => x.GetItemAuditPageAsync(criteria, ct), Times.Once);
        }
    }
}
