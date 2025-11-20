using GameTools.Server.Application.Common.Paging;

namespace GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage
{
    public sealed record ItemAuditPageCriteria(
        Pagination Pagination, 
        ItemAuditFilter? Filter,
        string SortBy = "Id",
        bool Desc = true);
}
