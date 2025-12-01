using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Items.Models;
using GameTools.Server.Application.Features.Items.Queries.GetItemsPage;

namespace GameTools.Server.Application.Tests.Features.Items.Queries.GetItemsPage
{
    public static class GetItemsPageTestData
    {
        public static ItemsPageCriteria BuildCriteria(
            Pagination? pagination = null,
            ItemsFilter? filter = null,
            string? sortBy = null,
            bool desc = true)
        {
            return new ItemsPageCriteria(
                Pagination: pagination ?? new Pagination(),
                Filter: filter,
                SortBy: sortBy ?? nameof(ItemReadModel.Id),
                Desc: desc
            );
        }
    }
}
