using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Coordinators.Audits;
using GameTools.Client.Wpf.Common.Coordinators.Items;
using GameTools.Client.Wpf.Common.State;

namespace GameTools.Client.Wpf.ViewModels.Items.Audits
{
    public sealed partial class ItemAuditPagingViewModel(
        IItemAuditPageSearchState itemAuditPageSearchState,
        IItemAuditsQueryCoordinator itemAuditsQueryCoordinator
        ) : ObservableObject, IRegionViewModel
    {
        public IItemAuditPageSearchState PageState => itemAuditPageSearchState;


        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task GoFirstPageAsync() => GoToPageAsync(1);

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task GoLastPageAsync() => GoToPageAsync(PageState.TotalPageNumber);

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task GoNextPageAsync() => GoToPageAsync(PageState.PageNumber + 1);

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task GoPreviewPageAsync() => GoToPageAsync(PageState.PageNumber - 1);

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task RefreshPage() => itemAuditsQueryCoordinator.RefreshAsync();

        private Task GoToPageAsync(int page)
        {
            if (page < 1 || page > PageState.TotalPageNumber) return Task.CompletedTask;
            return itemAuditsQueryCoordinator.GoToPageAsync(page);
        }


        public void OnRegionActivated(Parameters? parameters)
        {
        }

        public void OnRegionDeactivated()
        {
        }
    }
}
