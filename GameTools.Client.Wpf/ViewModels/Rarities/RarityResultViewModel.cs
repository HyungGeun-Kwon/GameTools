using CommunityToolkit.Mvvm.ComponentModel;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Rarities.Contracts;

namespace GameTools.Client.Wpf.ViewModels.Rarities
{
    public sealed partial class RarityResultViewModel(
        ISearchState<RarityEditModel> raritySearchState
        ): ObservableObject, IRegionViewModel
    {
        public ISearchState<RarityEditModel> State => raritySearchState;

        public void OnRegionActivated(Parameters? _) { }
        public void OnRegionDeactivated() { }
    }
}
