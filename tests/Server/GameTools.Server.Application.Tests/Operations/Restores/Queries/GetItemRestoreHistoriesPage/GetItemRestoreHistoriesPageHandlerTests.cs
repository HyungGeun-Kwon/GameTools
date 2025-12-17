using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Operations.Restores.Queries.GetItemRestoreHistoriesPage;
using Moq;

namespace GameTools.Server.Application.Tests.Operations.Restores.Queries.GetItemRestoreHistoriesPage
{
    public class GetItemRestoreHistoriesPageHandlerTests
    {
        private static GetItemRestoreHistoriesPageHandler CreateHandler(
            out Mock<IItemRestoreHistoryReadStore> storeMock)
        {
            storeMock = new Mock<IItemRestoreHistoryReadStore>();
            return new GetItemRestoreHistoriesPageHandler(storeMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_PagedResult_From_Store()
        {
            var handler = CreateHandler(out var storeMock);

            var criteria = new ItemRestoreHistoriesPageCriteria(
                Pagination: new Pagination(),
                Filter: new ItemRestoreHistoriesFilter(
                    Actors: ["tester"],
                    FromUtc: null,
                    ToUtc: null,
                    DryOnly: null),
                SortBy: nameof(ItemRestoreHistoriesReadModel.AsOfUtc),
                Desc: true);

            var query = new GetItemRestoreHistoriesPageQuery(criteria);
            var ct = CancellationToken.None;

            var items = new[]
            {
                new ItemRestoreHistoriesReadModel(
                    Id: Guid.NewGuid(),
                    AsOfUtc: DateTime.UtcNow.AddMinutes(-10),
                    CurrentUser: "tester",
                    DryRun: true,
                    StartedAtUtc: DateTime.UtcNow.AddMinutes(-10),
                    EndedAtUtc: DateTime.UtcNow.AddMinutes(-9),
                    AffectedCounts: "{\"items\":10}",
                    Notes: "dry run",
                    FiltersJson: "{}"),
                new ItemRestoreHistoriesReadModel(
                    Id: Guid.NewGuid(),
                    AsOfUtc: DateTime.UtcNow.AddMinutes(-5),
                    CurrentUser: "tester2",
                    DryRun: false,
                    StartedAtUtc: DateTime.UtcNow.AddMinutes(-5),
                    EndedAtUtc: null,
                    AffectedCounts: null,
                    Notes: null,
                    FiltersJson: null),
            };

            var pagedResult = new PagedResult<ItemRestoreHistoriesReadModel>(
                Items: items,
                PageNumber: 1,
                PageSize: 10,
                TotalCount: items.Length);

            storeMock
                .Setup(x => x.GetItemRestoreHistoriesPageAsync(criteria, ct))
                .ReturnsAsync(pagedResult);

            var result = await handler.Handle(query, ct);

            result.Should().BeSameAs(pagedResult);
            storeMock.Verify(x => x.GetItemRestoreHistoriesPageAsync(criteria, ct), Times.Once);
        }
    }
}
