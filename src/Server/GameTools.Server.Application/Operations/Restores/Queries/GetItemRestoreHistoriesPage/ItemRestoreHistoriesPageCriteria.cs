using GameTools.Server.Application.Common.Paging;

namespace GameTools.Server.Application.Operations.Restores.Queries.GetItemRestoreHistoriesPage
{
    public sealed record ItemRestoreHistoriesPageCriteria(
        Pagination Pagination, 
        ItemRestoreHistoriesFilter? Filter,
        string SortBy = "Id",
        bool Desc = true);
}
