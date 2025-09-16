using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Coordinators.Items;
using GameTools.Client.Wpf.Common.Names;
using GameTools.Client.Wpf.Common.State;

namespace GameTools.Client.Wpf.ViewModels.Items.Datas
{
    public sealed partial class ItemDataHostViewModel(
        IRegionService regionService,
        IItemsQueryCoordinator itemsQueryCoordinator,
        IItemsCommandCoordinator itemsCommandCoordinator,
        IItemsCsvCommandCoordinator itemsCsvCommandCoordinator,
        IItemPageSearchState itemPageSearchState
    ) : ObservableObject, IRegionViewModel
    {
        public IItemPageSearchState ItemPageSearchState => itemPageSearchState;

        [RelayCommand]
        private void QueryCancel() => itemsQueryCoordinator.CancelCommand.Execute(null);
        [RelayCommand]
        private void CommandCancel()
        {
            itemsCommandCoordinator.CancelCommand.Execute(null);
            itemsCsvCommandCoordinator.CancelCommand.Execute(null);
        }

        public void OnRegionActivated(Parameters? _)
        {
            regionService.SetView(RegionNames.Item_Data_HeaderRegion, RegionViewNames.Item_Data_HeaderView);
            regionService.SetView(RegionNames.Item_Data_ResultRegion, RegionViewNames.Item_Data_ResultView);
            regionService.SetView(RegionNames.Item_Data_PagingRegion, RegionViewNames.Item_Data_PagingView);
        }
        public void OnRegionDeactivated()
        {
            QueryCancelCommand.Execute(null);
            CommandCancelCommand.Execute(null);
        }
    }
}
