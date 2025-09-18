using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Features.Items.Audits.Coordinators;
using GameTools.Client.Wpf.Features.Items.Audits.State;
using GameTools.Client.Wpf.Shared.Names;

namespace GameTools.Client.Wpf.Features.Items.Audits.ViewModels
{
    public sealed partial class ItemAuditHostViewModel(
        IRegionService regionService,
        IItemAuditPageSearchState itemAuditPageSearchState,
        IItemAuditsQueryCoordinator itemAuditsQueryCoordinator
    ) : ObservableObject, IRegionViewModel
    {
        public IItemAuditPageSearchState ItemAuditPageSearchState => itemAuditPageSearchState;

        [RelayCommand]
        private void QueryCancel() => itemAuditsQueryCoordinator.CancelCommand.Execute(null);

        public void OnRegionActivated(Parameters? parameters)
        {
            regionService.SetView(RegionNames.Item_Audit_HeaderRegion, RegionViewNames.Item_Audit_HeaderView);
            regionService.SetView(RegionNames.Item_Audit_ResultRegion, RegionViewNames.Item_Audit_ResultView);
            regionService.SetView(RegionNames.Item_Audit_PagingRegion, RegionViewNames.Item_Audit_PagingView);
        }

        public void OnRegionDeactivated()
        {
            QueryCancelCommand.Execute(null);
        }
    }
}
