using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.UseCases.Rarities.CreateRarity;
using GameTools.Client.Application.UseCases.Rarities.DeleteRarity;
using GameTools.Client.Application.UseCases.Rarities.UpdateRarity;
using GameTools.Client.Domain.Rarities;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.Models.Rarities;
using GameTools.Client.Wpf.ViewModels.Rarities.Contracts;
using GameTools.Client.Wpf.ViewModels.Rarities.Mappers;

namespace GameTools.Client.Wpf.Common.Coordinators.Rarities
{
    public sealed partial class RaritiesCommandCoordinator(
        ISearchState<RarityEditModel> raritySearchState,
        UpdateRarityUseCase updateRarityUseCase,
        DeleteRarityUseCase deleteRarityUseCase,
        CreateRarityUseCase createRarityUseCase
    ) : CoordinatorBase(
            busyNotifier: raritySearchState.BusyState,
            busyPropertyName: nameof(raritySearchState.BusyState.CommandBusy),
            isBusy: () => raritySearchState.BusyState.CommandBusy,
            setBusy: v => raritySearchState.BusyState.CommandBusy = v
        ), IRaritiesCommandCoordinator
    {
        public Task<Rarity> CreateAsync(RarityCreateModel rarityEditModel, CancellationToken external = default)
            => RunExclusiveAsync(ct => createRarityUseCase.Handle(rarityEditModel.ToCreateRarityInput(), ct), external);

        public Task DeleteAsync(RarityEditModel rarityEditModel, CancellationToken external = default)
            => RunExclusiveAsync(ct => deleteRarityUseCase.Handle(rarityEditModel.ToDeleteRarityInput(), ct), external);

        public Task<Rarity> UpdateAsync(RarityEditModel rarityEditModel, CancellationToken external = default)
            => RunExclusiveAsync(ct => updateRarityUseCase.Handle(rarityEditModel.ToUpdateRarityInput(), ct), external);
    }
}
