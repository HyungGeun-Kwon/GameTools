using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Coordinators.RestoreHistories;
using GameTools.Client.Wpf.Common.Names;
using GameTools.Client.Wpf.Common.State;

namespace GameTools.Client.Wpf.ViewModels.RestoreHistories
{
    public sealed partial class RestoreHistoryHostViewModel(
        IRegionService regionService,
        IRestoreHistoriesQueryCoordinator restoreHistoriesQueryCoordinator,
        IRestoreHistoryPageSearchState restoreHistoryPageSearchState
        ) : ObservableObject, IRegionViewModel
    {
        public IRestoreHistoryPageSearchState RestoreHistoryPageSearchState => restoreHistoryPageSearchState;

        [RelayCommand]
        private void QueryCancel() => restoreHistoriesQueryCoordinator.CancelCommand.Execute(null);

        public void OnRegionActivated(Parameters? _)
        {
            regionService.SetView(RegionNames.Restore_SearchRegion, RegionViewNames.Restore_HeaderView);
            regionService.SetView(RegionNames.Restore_ResultRegion, RegionViewNames.Restore_ResultView);
            regionService.SetView(RegionNames.Restore_PagingRegion, RegionViewNames.Restore_PagingView);
        }

        public void OnRegionDeactivated()
        {
            QueryCancelCommand.Execute(null);
        }
    }
}
