using GameTools.Contracts.Common;
using GameTools.Contracts.Restores.Common;
using GameTools.Contracts.Restores.GetRestoresPage;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Restores.Queries.GetRestoreRunsPage;

namespace GameTools.Server.Api.Mapper
{
    internal static class RestoreApiMapper
    {
        internal static RestoreRunPageSearchCriteria ToCriteria(this RestoreRunPageRequest req)
            => new(
                new Pagination(req.PageNumber, req.PageSize), 
                new RestoreRunFilter(req.FromUtc, req.ToUtc, req.Actor, req.DryOnly));

        internal static RestoreRunResponse ToResponse(this RestoreRunReadModel readModel)
            => new
            (
                readModel.RestoreId, 
                readModel.AsOfUtc, 
                readModel.Actor, 
                readModel.DryRun, 
                readModel.StartedAtUtc, 
                readModel.EndedAtUtc, 
                readModel.AffectedCounts, 
                readModel.Notes, 
                readModel.FiltersJson
            );

        internal static PagedResponse<RestoreRunResponse> ToResponse(this PagedResult<RestoreRunReadModel> pagedResult)
            => new
            (
                pagedResult.Items.Select(ToResponse).ToList(),
                pagedResult.TotalCount,
                pagedResult.PageNumber,
                pagedResult.PageSize
            );

    }
}
