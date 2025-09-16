using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.Models.Restores;
using GameTools.Client.Application.UseCases.Restores.GetRestoreHistoryPage;
using GameTools.Contracts.Common;
using GameTools.Contracts.Restores.Common;
using GameTools.Contracts.Restores.GetRestoresPage;

namespace GameTools.Client.Infrastructure.Mapper
{
    public static class RestoreMapper
    {
        public static RestoreRunPageRequest ToContract(this GetRestoreHistoriesPageInput input)
            => new(
                input.Pagination.PageNumber,
                input.Pagination.PageSize,
                input.Filter?.FromUtc,
                input.Filter?.ToUtc,
                input.Filter?.Actor,
                input.Filter?.DryOnly
            );

        public static RestoreHistory ToClient(this RestoreRunResponse res)
            => new(
                res.RestoreId,
                res.AsOfUtc,
                res.Actor,
                res.DryRun,
                res.StartedAtUtc,
                res.EndedAtUtc,
                res.AffectedCounts,
                res.Notes,
                res.FiltersJson
            );

        public static PagedOutput<RestoreHistory> ToClient(this PagedResponse<RestoreRunResponse> p)
            => new(p.Items.Select(ToClient).ToList(), p.TotalCount, p.PageNumber, p.PageSize);

    }
}
