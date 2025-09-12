using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Coordinators.Items;
using GameTools.Client.Wpf.Common.State;

namespace GameTools.Client.Wpf.ViewModels.Items.Datas
{
    public sealed partial class ItemDataPagingViewModel(
        IItemPageSearchState itemPageSearchState,
        IItemsQueryCoordinator itemsQueryCoordinator)
        : ObservableObject, IRegionViewModel
    {
        public IItemPageSearchState PageState => itemPageSearchState;

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task GoFirstPageAsync() => GoToPageAsync(1);

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task GoLastPageAsync() => GoToPageAsync(PageState.TotalPageNumber);

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task GoNextPageAsync() => GoToPageAsync(PageState.PageNumber + 1);

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task GoPreviewPageAsync() => GoToPageAsync(PageState.PageNumber - 1);

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task RefreshPage() => itemsQueryCoordinator.RefreshAsync();

        private Task GoToPageAsync(int page)
        {
            if (page < 1 || page > PageState.TotalPageNumber) return Task.CompletedTask;
            return itemsQueryCoordinator.GoToPageAsync(page);
        }

        public void OnRegionActivated(Parameters? parameters) { }
        public void OnRegionDeactivated() { }
    }
}
