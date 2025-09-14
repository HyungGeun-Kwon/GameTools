using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.UseCases.Audits.GetItemAuditsPage;
using GameTools.Client.Wpf.Common.Enums;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Items.Mappers;

namespace GameTools.Client.Wpf.Common.Coordinators.Audits
{
    public sealed partial class ItemAuditsQueryCoordinator(
        IItemAuditPageSearchState itemAuditPageSearchState,
        GetItemAuditsPageUseCase getItemAuditPageUseCase
        ) : CoordinatorBase(
            busyNotifier: itemAuditPageSearchState.BusyState,
            busyPropertyName: nameof(itemAuditPageSearchState.BusyState.QueryBusy),
            isBusy: () => itemAuditPageSearchState.BusyState.QueryBusy,
            setBusy: v => itemAuditPageSearchState.BusyState.QueryBusy = v
        ), IItemAuditsQueryCoordinator
    {
        /// <summary>
        /// 동일한 필터로 재검색
        /// </summary>
        public Task RefreshAsync(CancellationToken external = default)
            => UpdatePageSearchState(itemAuditPageSearchState.ToGetItemAuditPageInput(), external);

        /// <summary>
        /// 지정한 페이지로 이동
        /// </summary>
        public Task GoToPageAsync(int page, CancellationToken external = default)
            => UpdatePageSearchState(itemAuditPageSearchState.GetItemAuditPageInputFromNewPage(page), external);

        /// <summary>
        /// 새로운 필터로 검색
        /// </summary>
        public Task SearchWithFilterAsync(
            int page, int pageSize,
            int? itemId, AuditActionType? action, DateTime? fromUtc, DateTime? toUtc,
            CancellationToken external = default)
            => UpdatePageSearchState(new(
                new(page, pageSize),
                new(itemId, action is null or AuditActionType.ALL ? null : action.Value.ToString(), fromUtc, toUtc)), external);

        private Task UpdatePageSearchState(GetItemAuditsPageInput input, CancellationToken external)
        {
            if (input.Pagination.PageNumber == 0 || input.Pagination.PageSize == 0) return Task.CompletedTask;

            return RunExclusiveAsync(async ct =>
            {
                var output = await getItemAuditPageUseCase.Handle(input, ct);
                // 성공적으로 Get 한 경우에 업데이트
                itemAuditPageSearchState.ReplacePageResults(output);
                itemAuditPageSearchState.ReplaceFilter(
                    input.Filter?.ItemId, input.Filter?.Action, input.Filter?.FromUtc, input.Filter?.ToUtc);
            }, external);
        }
    }
}