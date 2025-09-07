using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Coordinators.Items;
using GameTools.Client.Wpf.Common.Names;
using GameTools.Client.Wpf.Common.State;

namespace GameTools.Client.Wpf.ViewModels.Items
{
    public sealed partial class ItemHostViewModel(
        IRegionService regionService,
        IItemsQueryCoordinator itemsQueryCoordinator,
        IItemsCommandCoordinator itemsCommandCoordinator,
        IItemPageSearchState itemPageSearchState
        ) : ObservableObject, IRegionViewModel
    {
        public IItemPageSearchState ItemPageSearchState => itemPageSearchState;

        [RelayCommand]
        private void QueryCancel() => itemsQueryCoordinator.CancelCommand.Execute(null);
        [RelayCommand]
        private void CommandCancel() => itemsCommandCoordinator.CancelCommand.Execute(null);

        public void OnRegionActivated(Parameters? _)
        {
            regionService.SetView(RegionNames.Item_HeaderRegion, RegionViewNames.Item_HeaderView);
            regionService.SetView(RegionNames.Item_ResultRegion, RegionViewNames.Item_ResultView);
            regionService.SetView(RegionNames.Item_PagingRegion, RegionViewNames.Item_PagingView);
        }
        public void OnRegionDeactivated()
        {
            QueryCancelCommand.Execute(null);
            CommandCancelCommand.Execute(null);
        }
    }
}
