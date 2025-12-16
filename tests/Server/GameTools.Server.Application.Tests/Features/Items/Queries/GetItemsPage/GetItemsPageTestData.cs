using GameTools.Server.Application.Catalog.Items.Models;
using GameTools.Server.Application.Catalog.Items.Queries.GetItemsPage;
using GameTools.Server.Application.Common.Paging;

namespace GameTools.Server.Application.Tests.Catalog.Items.Queries.GetItemsPage
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
