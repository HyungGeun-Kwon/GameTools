using GameTools.Server.Application.Common.Paging;

namespace GameTools.Server.Application.Features.Restores.Queries.GetRestoreRunsPage
{
    public sealed record RestoreRunPageSearchCriteria(Pagination Pagination, RestoreRunFilter? Filter);
}
