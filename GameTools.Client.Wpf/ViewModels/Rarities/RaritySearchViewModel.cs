using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Application.UseCases.Rarities.GetAllRarities;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Rarities.Contracts;
using GameTools.Client.Wpf.ViewModels.Rarities.Mappers;

namespace GameTools.Client.Wpf.ViewModels.Rarities
{
    public sealed partial class RaritySearchViewModel(
        GetAllRaritiesUseCase getAllRaritiesUseCase,
        ISearchState<RarityEditModel> raritySearchState)
        : ObservableObject, IRegionViewModel
    {
        [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
        public async Task RaritySearch(CancellationToken token)
        {
            var result = await getAllRaritiesUseCase.Handle(token);
            raritySearchState.ReplaceResults(result.ToEditModels());
        }

        public void OnRegionActivated(Parameters? _) { }
        public void OnRegionDeactivated() { }
    }
}
