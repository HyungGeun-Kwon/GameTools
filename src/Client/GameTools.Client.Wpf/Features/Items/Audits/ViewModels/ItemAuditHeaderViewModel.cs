using CommunityToolkit.Mvvm.ComponentModel;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Shared.Names;

namespace GameTools.Client.Wpf.Features.Items.Audits.ViewModels
{
    public sealed class ItemAuditHeaderViewModel(IRegionService regionService) 
        : ObservableObject, IRegionViewModel
    {
        public void OnRegionActivated(Parameters? parameters)
        {
            regionService.SetView(RegionNames.Item_Audit_SearchRegion, RegionViewNames.Item_Audit_SearchView);
            regionService.SetView(RegionNames.Item_Audit_CommandRegion, RegionViewNames.Item_Audit_CommandView);
        }

        public void OnRegionDeactivated()
        {
        }
    }
}
