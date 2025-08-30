using CommunityToolkit.Mvvm.ComponentModel;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;

namespace GameTools.Client.Wpf.ViewModels.Items
{
    public sealed partial class ItemResultViewModel : ObservableObject, IRegionViewModel
    {
        public void OnRegionActivated(Parameters? _)
        {
        }

        public void OnRegionDeactivated()
        {
        }
    }
}
