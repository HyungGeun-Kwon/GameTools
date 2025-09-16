using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Coordinators.Items;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Items.Contracts;

namespace GameTools.Client.Wpf.ViewModels.Items.Datas
{
    public sealed partial class ItemDataResultViewModel(
        IItemPageSearchState itemPageSearchState,
        IItemsQueryCoordinator itemsQueryCoordinator,
        IItemsCommandCoordinator itemsCommandCoordinator,
        RarityLookupViewModel rarityLookupViewModel
        ) : ObservableObject, IRegionViewModel
    {
        public RarityLookupViewModel RarityLookup => rarityLookupViewModel;

        public IItemPageSearchState PageState => itemPageSearchState;

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task UpdateItemAsync(ItemEditModel? model)
        {
            if (model is null) return;
            if (model.HasErrors) return;
            if (!model.IsDirty) return;

            var newModel = await itemsCommandCoordinator.UpdateAsync(model);
            model.FinishEdit(newModel.RowVersionBase64);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task DeleteItemAsync(ItemEditModel? model)
        {
            if (model is null) return;

            var btnResult = MessageBox.Show("Are you sure you want to delete it?", "", MessageBoxButton.YesNo);
            if (btnResult != MessageBoxResult.Yes) return;

            try
            {
                await itemsCommandCoordinator.DeleteAsync(model);
                await itemsQueryCoordinator.RefreshAsync();
            }
            catch (OperationCanceledException) { }
        }

        [RelayCommand]
        private void RevertItem(ItemEditModel? model)
        {
            if (model is null) return;
            model.RevertToSaved();
        }

        public async void OnRegionActivated(Parameters? _)
            => await RarityLookup.LoadCommand.ExecuteAsync(null);

        public void OnRegionDeactivated() => RarityLookup.LoadCancelCommand.Execute(null);
    }
}
