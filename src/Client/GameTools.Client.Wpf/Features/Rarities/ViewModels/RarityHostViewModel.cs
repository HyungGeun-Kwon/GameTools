using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Features.Rarities.Coordinators;
using GameTools.Client.Wpf.Features.Rarities.Models;
using GameTools.Client.Wpf.Shared.Names;
using GameTools.Client.Wpf.Shared.State;

namespace GameTools.Client.Wpf.Features.Rarities.ViewModels
{
    public sealed partial class RarityHostViewModel(
        IRegionService regionService,
        IRaritiesQueryCoordinator raritiesQueryCoordinator,
        IRaritiesCommandCoordinator raritiesCommandCoordinator,
        ISearchState<RarityEditModel> raritySearchState
        ) : ObservableObject, IRegionViewModel
    {
        public ISearchState<RarityEditModel> RaritySearchState => raritySearchState;

        [RelayCommand]
        private void QueryCancel() => raritiesQueryCoordinator.CancelCommand.Execute(null);
        [RelayCommand]
        private void CommandCancel() => raritiesCommandCoordinator.CancelCommand.Execute(null);

        public void OnRegionActivated(Parameters? _)
        {
            regionService.SetView(RegionNames.Rarity_SearchRegion, RegionViewNames.Rarity_HeaderView);
            regionService.SetView(RegionNames.Rarity_ResultRegion, RegionViewNames.Rarity_ResultView);
        }

        public void OnRegionDeactivated()
        {
            QueryCancelCommand.Execute(null);
            CommandCancelCommand.Execute(null);
        }
    }
}
