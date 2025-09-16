using GameTools.Client.Application.UseCases.Restores.GetRestoreHistoryPage;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.RestoreHistories.Mappers;

namespace GameTools.Client.Wpf.Common.Coordinators.RestoreHistories
{
    public sealed partial class RestoreHistoriesQueryCoordinator(
        IRestoreHistoryPageSearchState restoreHistoryPageSearchState,
        GetRestoreHistoriesPageUseCase getRestoresPageUseCase
    ) : CoordinatorBase(
            busyNotifier: restoreHistoryPageSearchState.BusyState,
            busyPropertyName: nameof(restoreHistoryPageSearchState.BusyState.QueryBusy),
            isBusy: () => restoreHistoryPageSearchState.BusyState.QueryBusy,
            setBusy: v => restoreHistoryPageSearchState.BusyState.QueryBusy = v
        ), IRestoreHistoriesQueryCoordinator
    {
        /// <summary>
        /// 동일한 필터로 재검색
        /// </summary>
        public Task RefreshAsync(CancellationToken external = default)
            => UpdatePageSearchState(restoreHistoryPageSearchState.ToGetRestoreHistoryPageInput(), external);

        /// <summary>
        /// 지정한 페이지로 이동
        /// </summary>
        public Task GoToPageAsync(int page, CancellationToken external = default)
            => UpdatePageSearchState(restoreHistoryPageSearchState.GetrestoreHistoryPageInputFromNewPage(page), external);

        /// <summary>
        /// 새로운 필터로 검색
        /// </summary>
        public Task SearchWithFilterAsync(
            int page, int pageSize,
            DateTime? fromUtcFilter, DateTime? toUtcFilter, string? actorFilter, bool? dryOnlyFilter,
            CancellationToken external = default)
            => UpdatePageSearchState(new(new(page, pageSize), new(fromUtcFilter, toUtcFilter, actorFilter, dryOnlyFilter)), external);

        private Task UpdatePageSearchState(GetRestoreHistoriesPageInput input, CancellationToken external)
        {
            if (input.Pagination.PageNumber == 0 || input.Pagination.PageSize == 0) return Task.CompletedTask;

            return RunExclusiveAsync(async ct =>
            {
                var output = await getRestoresPageUseCase.Handle(input, ct);
                // 성공적으로 Get 한 경우에 업데이트
                restoreHistoryPageSearchState.ReplacePageResults(output);
                restoreHistoryPageSearchState.ReplaceFilter(
                    input.Filter?.FromUtc, input.Filter?.ToUtc, input.Filter?.Actor, input.Filter?.DryOnly);
            }, external);
        }
    }
}
