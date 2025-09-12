using CommunityToolkit.Mvvm.ComponentModel;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Names;

namespace GameTools.Client.Wpf.ViewModels.Items.Datas
{
    public sealed partial class ItemDataHeaderViewModel(IRegionService regionService) 
        : ObservableObject, IRegionViewModel
    {
        public void OnRegionActivated(Parameters? _)
        {
            regionService.SetView(RegionNames.Item_Data_SearchRegion, RegionViewNames.Item_Data_SearchView);
            regionService.SetView(RegionNames.Item_Data_CommandRegion, RegionViewNames.Item_Data_CommandView);
        }

        public void OnRegionDeactivated() { }
    }
}
