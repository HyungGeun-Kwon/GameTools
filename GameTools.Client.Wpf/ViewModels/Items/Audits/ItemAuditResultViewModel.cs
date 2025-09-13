using CommunityToolkit.Mvvm.ComponentModel;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Coordinators.Audits;
using GameTools.Client.Wpf.Common.State;

namespace GameTools.Client.Wpf.ViewModels.Items.Audits
{
    public sealed class ItemAuditResultViewModel(
        IItemAuditPageSearchState itemAuditPageSearchState)
        : ObservableObject, IRegionViewModel
    {
        public IItemAuditPageSearchState PageState => itemAuditPageSearchState;
        public void OnRegionActivated(Parameters? parameters)
        {
        }

        public void OnRegionDeactivated()
        {
        }
    }
}
