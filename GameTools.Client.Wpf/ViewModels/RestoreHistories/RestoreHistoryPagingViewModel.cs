using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Coordinators.RestoreHistories;
using GameTools.Client.Wpf.Common.State;

namespace GameTools.Client.Wpf.ViewModels.RestoreHistories
{
    public sealed partial class RestoreHistoryPagingViewModel(
        IRestoreHistoryPageSearchState restoreHistoryPageSearchState,
        IRestoreHistoriesQueryCoordinator restoreHistoriesQueryCoordinator
    ) : ObservableObject, IRegionViewModel
    {
        public IRestoreHistoryPageSearchState PageState => restoreHistoryPageSearchState;

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task GoFirstPageAsync() => GoToPageAsync(1);

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task GoLastPageAsync() => GoToPageAsync(PageState.TotalPageNumber);

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task GoNextPageAsync() => GoToPageAsync(PageState.PageNumber + 1);

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task GoPreviewPageAsync() => GoToPageAsync(PageState.PageNumber - 1);

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task RefreshPage() => restoreHistoriesQueryCoordinator.RefreshAsync();

        private Task GoToPageAsync(int page)
        {
            if (page < 1 || page > PageState.TotalPageNumber) return Task.CompletedTask;
            return restoreHistoriesQueryCoordinator.GoToPageAsync(page);
        }

        public void OnRegionActivated(Parameters? parameters) { }
        public void OnRegionDeactivated() { }
    }
}
