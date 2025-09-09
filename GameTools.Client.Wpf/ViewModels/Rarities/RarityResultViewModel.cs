using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Coordinators.Rarities;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Rarities.Contracts;

namespace GameTools.Client.Wpf.ViewModels.Rarities
{
    public sealed partial class RarityResultViewModel(
        ISearchState<RarityEditModel> raritySearchState,
        IRaritiesQueryCoordinator raritiesQueryCoordinator,
        IRaritiesCommandCoordinator raritiesCommandCoordinator
        ) : ObservableObject, IRegionViewModel
    {
        public ISearchState<RarityEditModel> State => raritySearchState;

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task UpdateRarityAsync(RarityEditModel? model)
        {
            if (model is null) return;
            if (model.HasErrors) return;
            if (!model.IsDirty) return;

            var newModel = await raritiesCommandCoordinator.UpdateAsync(model);
            model.FinishEdit(newModel.RowVersionBase64);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task DeleteRarityAsync(RarityEditModel? model)
        {
            if (model is null) return;

            var btnResult = MessageBox.Show("Are you sure you want to delete it?", "", MessageBoxButton.YesNo);
            if (btnResult != MessageBoxResult.Yes) return;

            try
            {
                await raritiesCommandCoordinator.DeleteAsync(model);
                await raritiesQueryCoordinator.SearchAllAsync();
            }
            catch(OperationCanceledException) { }
        }

        [RelayCommand]
        private void RevertRarity(RarityEditModel? model)
        {
            if (model is null) return;
            model.RevertToSaved();
        }

        public void OnRegionActivated(Parameters? _) { }
        public void OnRegionDeactivated() { }
    }
}
