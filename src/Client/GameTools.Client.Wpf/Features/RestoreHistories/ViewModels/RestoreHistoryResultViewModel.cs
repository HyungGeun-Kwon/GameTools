using CommunityToolkit.Mvvm.ComponentModel;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Features.RestoreHistories.State;

namespace GameTools.Client.Wpf.Features.RestoreHistories.ViewModels
{
    public sealed partial class RestoreHistoryResultViewModel(
        IRestoreHistoryPageSearchState restoreHistoryPageSearchState
    ) : ObservableObject, IRegionViewModel
    {
        public IRestoreHistoryPageSearchState PageState => restoreHistoryPageSearchState;

        public void OnRegionActivated(Parameters? _) { }
        public void OnRegionDeactivated() { }
    }
}
