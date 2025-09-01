using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Application.UseCases.Rarities.DeleteRarity;
using GameTools.Client.Application.UseCases.Rarities.GetAllRarities;
using GameTools.Client.Application.UseCases.Rarities.UpdateRarity;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Rarities.Contracts;
using GameTools.Client.Wpf.ViewModels.Rarities.Mappers;

namespace GameTools.Client.Wpf.ViewModels.Rarities
{
    public sealed partial class RarityResultViewModel(
        ISearchState<RarityEditModel> raritySearchState,
        GetAllRaritiesUseCase getAllRaritiesUseCase,
        UpdateRarityUseCase updateRarityUseCase,
        DeleteRarityUseCase deleteRarityuseCase
        ) : ObservableObject, IRegionViewModel
    {
        public ISearchState<RarityEditModel> State => raritySearchState;

        [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
        private async Task UpdateRowAsync(RarityEditModel? model, CancellationToken ct)
        {
            if (model is null) return;
            if (model.HasErrors) return;
            if (!model.IsDirty) return;

            State.IsBusy = true;
            try
            {
                var newModel = await updateRarityUseCase.Handle(model.ToUpdateRarityInput(), ct);

                model.AcceptChanges();
                model.FinishEdit(newModel.RowVersionBase64);
            }
            finally
            {
                State.IsBusy = false;
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false, IncludeCancelCommand = true)]
        private async Task DeleteRowAsync(RarityEditModel? model, CancellationToken ct)
        {
            if (model is null) return;

            var btnResult = MessageBox.Show("Are you sure you want to delete it?", "", MessageBoxButton.YesNo);
            if (btnResult != MessageBoxResult.Yes) return;

            State.IsBusy = true;
            try
            {
                await deleteRarityuseCase.Handle(model.ToDeleteRarityInput(), ct);

                var result = await getAllRaritiesUseCase.Handle(ct);
                raritySearchState.ReplaceResults(result.ToEditModels());
            }
            finally
            {
                State.IsBusy = false;
            }
        }

        public void OnRegionActivated(Parameters? _) { }
        public void OnRegionDeactivated() { }
    }
}
