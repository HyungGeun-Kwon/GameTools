using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.DialogServices;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Wpf.Common.Coordinators.Items;
using GameTools.Client.Wpf.Common.Names;
using GameTools.Client.Wpf.Common.State;

namespace GameTools.Client.Wpf.ViewModels.Items
{
    public sealed partial class ItemCommandViewModel(
        IDialogService dialogService,
        IItemPageSearchState itemPageSearchState,
        IItemsQueryCoordinator itemsQueryCoordinator,
        IItemsCsvCommandCoordinator itemsCsvCommandCoordinator
        ) : ObservableObject, IRegionViewModel
    {
        [RelayCommand]
        private void AddItem()
            => dialogService.ShowDialog(DialogViewNames.Item_EditDialog, null, ItemEditDialogClosed);

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task DeleteAllResult()
        {
            var btnResult = MessageBox.Show("Are you sure you want to delete it?", "", MessageBoxButton.YesNo);
            if (btnResult != MessageBoxResult.Yes) return Task.CompletedTask;
            return itemsCsvCommandCoordinator.ImportAndAllResultDeleteAsync();
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
        private Task BulkInsert()
            => itemsCsvCommandCoordinator.ImportAndBulkInsertAsync();

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task BulkUpdate()
            => itemsCsvCommandCoordinator.ImportAndBulkUpdateAsync();

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task BulkDelete()
            => itemsCsvCommandCoordinator.ImportAndBulkDeleteAsync();

        private async void ItemEditDialogClosed(IDialogResult? result)
        {
            if (result?.ButtonResult != ButtonResult.OK) return;

            try
            {
                if (itemPageSearchState.Results.Count == 0) return;

                // 가장 최근에 검색했던 방식으로 검색.
                await itemsQueryCoordinator.RefreshAsync();
            }
            catch (OperationCanceledException) { }
        }

        public void OnRegionActivated(Parameters? _) { }
        public void OnRegionDeactivated() { }
    }
}
