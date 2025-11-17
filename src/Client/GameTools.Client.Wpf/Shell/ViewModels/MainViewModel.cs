using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Shared.Names;

namespace GameTools.Client.Wpf.Shell.ViewModels
{
    public partial class MainViewModel(IRegionService regionService) : ObservableObject
    {
        [RelayCommand]
        private void Loaded()
        {
            regionService.SetView(RegionNames.Main_LeftNavigationRegion, RegionViewNames.Main_EntityNavigationView);
        }
    }
}
