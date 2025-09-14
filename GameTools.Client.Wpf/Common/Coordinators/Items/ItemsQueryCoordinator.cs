using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.UseCases.Items.GetItemsPage;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Items.Mappers;

namespace GameTools.Client.Wpf.Common.Coordinators.Items
{
    public sealed partial class ItemsQueryCoordinator(
        IItemPageSearchState itemPageSearchState,
        GetItemsPageUseCase getItemsPageUseCase
    ) : CoordinatorBase(
            busyNotifier: itemPageSearchState.BusyState,
            busyPropertyName: nameof(itemPageSearchState.BusyState.QueryBusy),
            isBusy: () => itemPageSearchState.BusyState.QueryBusy,
            setBusy: v => itemPageSearchState.BusyState.QueryBusy = v
        ), IItemsQueryCoordinator
    {
        /// <summary>
        /// 동일한 필터로 재검색 (Delete, Create 이후 등)
        /// </summary>
        public Task RefreshAsync(CancellationToken external = default)
            => UpdatePageSearchState(itemPageSearchState.ToGetItemPageInput(), external);

        /// <summary>
        /// 지정한 페이지로 이동
        /// </summary>
        public Task GoToPageAsync(int page, CancellationToken external = default)
            => UpdatePageSearchState(itemPageSearchState.GetItemPageInputFromNewPage(page), external);

        /// <summary>
        /// 새로운 필터로 검색
        /// </summary>
        public Task SearchWithFilterAsync(
            int page, int pageSize,
            string? nameFilter, byte? rarityIdFilter,
            CancellationToken external = default)
            => UpdatePageSearchState(new(new(page, pageSize), new(nameFilter, rarityIdFilter)), external);

        private Task UpdatePageSearchState(GetItemsPageInput input, CancellationToken external)
        {
            if (input.Pagination.PageNumber == 0 || input.Pagination.PageSize == 0) return Task.CompletedTask;

            return RunExclusiveAsync(async ct =>
            {
                var output = await getItemsPageUseCase.Handle(input, ct);
                // 성공적으로 Get 한 경우에 업데이트
                itemPageSearchState.ReplacePageResults(output.ToPagedItemEditModel());
                itemPageSearchState.ReplaceFilter(input.Filter?.NameSearch, input.Filter?.RarityId);
            }, external);
        }
    }
}
