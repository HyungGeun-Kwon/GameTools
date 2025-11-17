using GameTools.Client.Application.UseCases.Rarities.GetAllRarities;
using GameTools.Client.Wpf.Features.Rarities.Mappers;
using GameTools.Client.Wpf.Features.Rarities.Models;
using GameTools.Client.Wpf.Shared.Coordinators;
using GameTools.Client.Wpf.Shared.State;

namespace GameTools.Client.Wpf.Features.Rarities.Coordinators
{
    public sealed partial class RaritiesQueryCoordinator(
        ISearchState<RarityEditModel> raritySearchState,
        GetAllRaritiesUseCase getRaritiesUseCase
    ) : CoordinatorBase(
            busyNotifier: raritySearchState.BusyState,
            busyPropertyName: nameof(raritySearchState.BusyState.QueryBusy),
            isBusy: () => raritySearchState.BusyState.QueryBusy,
            setBusy: v => raritySearchState.BusyState.QueryBusy = v
        ), IRaritiesQueryCoordinator
    {
        public Task SearchAllAsync(CancellationToken external = default)
            => RunExclusiveAsync(async ct =>
            {
                var result = await getRaritiesUseCase.Handle(ct);
                raritySearchState.ReplaceResults(result.ToEditModels());
            }, external);
    }
}
