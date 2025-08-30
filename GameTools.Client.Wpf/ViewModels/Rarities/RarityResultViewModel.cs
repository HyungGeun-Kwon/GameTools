using CommunityToolkit.Mvvm.ComponentModel;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;

namespace GameTools.Client.Wpf.ViewModels.Rarities
{
    public sealed partial class RarityResultViewModel : ObservableObject, IRegionViewModel
    {

        public void OnRegionActivated(Parameters? _) { }
        public void OnRegionDeactivated() { }
    }
}
