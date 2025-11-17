using GameTools.Server.Application.Common.Paging;

namespace GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage
{
    public sealed record ItemAuditPageSearchCriteria(Pagination Pagination, ItemAuditFilter? Filter);
}
