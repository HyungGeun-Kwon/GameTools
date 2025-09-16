using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Restores.Queries.GetRestoreRunsPage;

namespace GameTools.Server.Application.Abstractions.Stores.ReadStore
{
    public interface IRestoreRunReadStore
    {
        Task<PagedResult<RestoreRunReadModel>> GetRestoreRunsPageAsync(RestoreRunPageSearchCriteria criteria, CancellationToken ct);
    }
}
