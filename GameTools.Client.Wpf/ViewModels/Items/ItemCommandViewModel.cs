using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.DialogServices;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Application.Ports;
using GameTools.Client.Application.UseCases.Items.BulkInsertItems;
using GameTools.Client.Application.UseCases.Items.BulkUpdateItems;
using GameTools.Client.Wpf.Common.Coordinators.Items;
using GameTools.Client.Wpf.Common.FilePickers;
using GameTools.Client.Wpf.Common.Names;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Items.Contracts;

namespace GameTools.Client.Wpf.ViewModels.Items
{
    public sealed partial class ItemCommandViewModel(
        IDialogService dialogService,
        IItemPageSearchState itemPageSearchState,
        IItemsQueryCoordinator itemsQueryCoordinator,
        IItemsCommandCoordinator itemsCommandCoordinator,
        IItemsCsvCommandCoordinator itemsCsvCoordinator,
        IFilePickerService filePickerService,
        ICsvSerializer csvSerializer) : ObservableObject, IRegionViewModel
    {
        [RelayCommand]
        private void AddItem()
        {
            dialogService.ShowDialog(DialogViewNames.Item_EditDialog, null, ItemEditDialogClosed);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task ExportCsv()
        {
            if (itemPageSearchState.Results.Count == 0)
            {
                MessageBox.Show("No results to export.");
                return;
            }

            await itemsCsvCoordinator.ExportPageResultsAsync(itemPageSearchState.Results);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task ExportBulkInsertBaseCsv()
            => itemsCsvCoordinator.ExportBulkInsertTemplateAsync();

        [RelayCommand(AllowConcurrentExecutions = false)]
        private Task ExportBulkUpdateBaseCsv()
            => itemsCsvCoordinator.ExportBulkUpdateTemplateAsync();

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task BulkInsert()
        {
            IReadOnlyList<string> insertCsvPaths = await filePickerService.OpenFilesAsync("Select Insert CSV", [FileDialogFilters.Csv]);
            if (insertCsvPaths.Count == 0) return;

            List<BulkInsertItemInputRow> rows = [];

            foreach (var path in insertCsvPaths)
            {
                await using var fs = File.OpenRead(path);
                rows.AddRange(await csvSerializer.ReadAsync<BulkInsertItemInputRow>(fs));
            }

            BulkInsertItemsInput input = new(rows);
            var output = await itemsCommandCoordinator.BulkInsertAsync(input);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task BulkUpdate()
        {
            IReadOnlyList<string> insertCsvPaths = await filePickerService.OpenFilesAsync("Select Update CSV", [FileDialogFilters.Csv]);
            if (insertCsvPaths.Count == 0) return;

            List<BulkUpdateItemInputRow> rows = [];

            foreach (var path in insertCsvPaths)
            {
                await using var fs = File.OpenRead(path);
                rows.AddRange(await csvSerializer.ReadAsync<BulkUpdateItemInputRow>(fs));
            }

            BulkUpdateItemsInput input = new(rows);
            var output = await itemsCommandCoordinator.BulkUpdateAsync(input);
        }

        private async Task<FileStream?> CreateSaveFileStreamFromFilePicker()
        {
            var path = await filePickerService.SaveFileAsync("Save CSV", [FileDialogFilters.Csv, FileDialogFilters.All], "", "csv");
            if (string.IsNullOrEmpty(path)) return null;

            return File.Create(path);
        }

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
