using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Application.UseCases.Items.DeleteItem;
using GameTools.Client.Application.UseCases.Items.GetItemsPage;
using GameTools.Client.Application.UseCases.Items.UpdateItem;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Items.Contracts;
using GameTools.Client.Wpf.ViewModels.Items.Mappers;

namespace GameTools.Client.Wpf.ViewModels.Items
{
    public sealed partial class ItemResultViewModel(
        IItemPageSearchState itemPagedSearchState,
        GetItemsPageUseCase getItemsPageUseCase,
        UpdateItemUseCase updateItemUseCase,
        DeleteItemUseCase deleteItemUseCase,
        RarityLookupViewModel rarityLookupViewModel
        ) : ObservableObject, IRegionViewModel
    {
        [ObservableProperty]
        private RarityLookupViewModel _rarityLookup = rarityLookupViewModel;

        public IItemPageSearchState PageState => itemPagedSearchState;

        [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
        private async Task UpdateItemAsync(ItemEditModel? model, CancellationToken ct)
        {
            if (model is null) return;
            if (model.HasErrors) return;
            if (!model.IsDirty) return;

            PageState.IsBusy = true;
            try
            {
                var newModel = await updateItemUseCase.Handle(model.ToUpdateItemInput(), ct);

                model.FinishEdit(newModel.RowVersionBase64);
            }
            finally
            {
                PageState.IsBusy = false;
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false, IncludeCancelCommand = true)]
        private async Task DeleteItemAsync(ItemEditModel? model, CancellationToken ct)
        {
            if (model is null) return;

            var btnResult = MessageBox.Show("Are you sure you want to delete it?", "", MessageBoxButton.YesNo);
            if (btnResult != MessageBoxResult.Yes) return;

            PageState.IsBusy = true;
            try
            {
                await deleteItemUseCase.Handle(model.ToDeleteItemInput(), ct);
                
                var itemPageOutput = await getItemsPageUseCase.Handle(PageState.ToGetItemPageInput(), ct);
                PageState.ReplacePageResults(itemPageOutput.ToPagedItemEditModel());
            }
            finally
            {
                PageState.IsBusy = false;
            }
        }

        [RelayCommand]
        private void RevertItem(ItemEditModel? model)
        {
            if (model is null) return;
            model.RevertToSaved();
        }

        public async void OnRegionActivated(Parameters? _)
            => await RarityLookup.LoadCommand.ExecuteAsync(null);

        public void OnRegionDeactivated() { }
    }
}
