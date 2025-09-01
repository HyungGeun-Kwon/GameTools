using CommunityToolkit.Mvvm.ComponentModel;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Names;

namespace GameTools.Client.Wpf.ViewModels.Rarities
{
    public sealed partial class RarityHostViewModel(IRegionService regionService) : ObservableObject, IRegionViewModel
    {
        public void OnRegionActivated(Parameters? _)
        {
            regionService.SetView(RegionNames.Rarity_SearchRegion, RegionViewNames.Rarity_HeaderView);
            regionService.SetView(RegionNames.Rarity_ResultRegion, RegionViewNames.Rarity_ResultView);
        }

        public void OnRegionDeactivated() { }
    }
}
