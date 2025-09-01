using CommunityToolkit.Mvvm.ComponentModel;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Names;

namespace GameTools.Client.Wpf.ViewModels.Items
{
    public sealed partial class ItemHostViewModel(IRegionService regionService) : ObservableObject, IRegionViewModel
    {

        public void OnRegionActivated(Parameters? _)
        {
            regionService.SetView(RegionNames.Item_SearchRegion, RegionViewNames.Item_HeaderView);
            regionService.SetView(RegionNames.Item_ResultRegion, RegionViewNames.Item_ResultView);
        }
        public void OnRegionDeactivated() { }
    }
}
