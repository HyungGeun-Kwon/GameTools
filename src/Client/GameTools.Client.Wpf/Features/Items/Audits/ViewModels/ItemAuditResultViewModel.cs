using CommunityToolkit.Mvvm.ComponentModel;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Features.Items.Audits.State;

namespace GameTools.Client.Wpf.Features.Items.Audits.ViewModels
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
