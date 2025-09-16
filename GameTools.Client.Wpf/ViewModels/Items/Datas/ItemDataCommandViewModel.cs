using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.DialogServices;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Coordinators.Items;
using GameTools.Client.Wpf.Common.Names;
using GameTools.Client.Wpf.Common.State;

namespace GameTools.Client.Wpf.ViewModels.Items.Datas
{
    public sealed partial class ItemDataCommandViewModel(
        IDialogService dialogService,
        IItemPageSearchState itemPageSearchState,
        IItemsQueryCoordinator itemsQueryCoordinator,
        IItemsCommandCoordinator itemsCommandCoordinator,
        IItemsCsvCommandCoordinator itemsCsvCommandCoordinator
        ) : ObservableObject, IRegionViewModel
    {
        [RelayCommand]
        private void AddItem()
            => dialogService.ShowDialog(DialogViewNames.Item_EditDialog, null, ItemEditDialogClosed);

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task DeleteAllResult()
        {
            var btnResult = MessageBox.Show("Are you sure you want to delete it?", "", MessageBoxButton.YesNo);
            if (btnResult != MessageBoxResult.Yes) return;
            await itemsCommandCoordinator.BulkDeleteAsync(itemPageSearchState.Results);
            await RefreshIfNeedAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task ExportCsv()
        {
            if (itemPageSearchState.Results.Count == 0)
            {
                MessageBox.Show("No results to export.");
                return Task.CompletedTask;
            }

            return itemsCsvCommandCoordinator.ExportPageResultsAsync(itemPageSearchState.Results);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task ExportBulkInsertBaseCsv()
            => itemsCsvCommandCoordinator.ExportBulkInsertTemplateAsync();

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task ExportBulkUpdateBaseCsv()
            => itemsCsvCommandCoordinator.ExportBulkUpdateTemplateAsync();

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task ExportBulkDeleteBaseCsv()
            => itemsCsvCommandCoordinator.ExportBulkDeleteTemplateAsync();

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task BulkInsert()
        {
            await itemsCsvCommandCoordinator.ImportAndBulkInsertAsync();
            await RefreshIfNeedAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task BulkUpdate()
        {
            await itemsCsvCommandCoordinator.ImportAndBulkUpdateAsync();
            await RefreshIfNeedAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task BulkDelete()
        {
            await itemsCsvCommandCoordinator.ImportAndBulkDeleteAsync();
            await RefreshIfNeedAsync();
        }

        private async void ItemEditDialogClosed(IDialogResult? result)
        {
            if (result?.ButtonResult != ButtonResult.OK) return;
            await RefreshIfNeedAsync();
        }

        private Task RefreshIfNeedAsync()
        {
            if (itemPageSearchState.Results.Count == 0) return Task.CompletedTask;

            // 가장 최근에 검색했던 방식으로 검색.
            return itemsQueryCoordinator.RefreshAsync();
        }

        public void OnRegionActivated(Parameters? _) { }
        public void OnRegionDeactivated() { }
    }
}
