using CommunityToolkit.Mvvm.ComponentModel;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Shared.Components.Tabs;
using GameTools.Client.Wpf.Shared.Names;

namespace GameTools.Client.Wpf.Features.Items.Host.ViewModels
{
    public sealed partial class ItemHostViewModel : ObservableObject, IRegionViewModel
    {
        public TabsHostViewModel TabsHostViewModel { get; }

        public ItemHostViewModel(TabsHostViewModel tabsHostViewModel)
        {
            tabsHostViewModel.AddTab(new RegionTabItem("Data", RegionViewNames.Item_Data_HostView), true);
            tabsHostViewModel.AddTab(new RegionTabItem("Audit", RegionViewNames.Item_Audit_HostView));
            TabsHostViewModel = tabsHostViewModel;

        }

        public void OnRegionActivated(Parameters? _)
        {
        }
        public void OnRegionDeactivated() { }
    }
}
