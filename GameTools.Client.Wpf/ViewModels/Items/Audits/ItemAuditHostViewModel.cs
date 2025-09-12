using CommunityToolkit.Mvvm.ComponentModel;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Names;

namespace GameTools.Client.Wpf.ViewModels.Items.Audits
{
    public sealed class ItemAuditHostViewModel(
        IRegionService regionService
    ) : ObservableObject, IRegionViewModel
    {
        public void OnRegionActivated(Parameters? parameters)
        {
            regionService.SetView(RegionNames.Item_Audit_HeaderRegion, RegionViewNames.Item_Audit_HeaderView);
            regionService.SetView(RegionNames.Item_Audit_ResultRegion, RegionViewNames.Item_Audit_ResultView);
            regionService.SetView(RegionNames.Item_Audit_PagingRegion, RegionViewNames.Item_Audit_PagingView);
        }

        public void OnRegionDeactivated()
        {
        }
    }
}
