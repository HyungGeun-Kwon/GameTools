using GameTools.Server.Application.Common.Paging;

namespace GameTools.Server.Application.Catalog.Items.Queries.GetItemsPage
{
    public sealed record ItemsPageCriteria(
        Pagination Pagination,
        ItemsFilter? Filter,
        string SortBy = "Id",
        bool Desc = true
    );
}
