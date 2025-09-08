using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.DialogServices;
using DotNetHelper.MsDiKit.RegionServices;
using GameTools.Client.Application.Ports;
using GameTools.Client.Wpf.Common.Coordinators.Items;
using GameTools.Client.Wpf.Common.FilePickers;
using GameTools.Client.Wpf.Common.Names;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Items.Contracts;
using GameTools.Contracts.Items.BulkInsertItems;
using GameTools.Contracts.Items.BulkUpdateItems;

namespace GameTools.Client.Wpf.ViewModels.Items
{
    public sealed partial class ItemCommandViewModel(
        IDialogService dialogService,
        IItemPageSearchState itemPageSearchState,
        IItemsQueryCoordinator itemsQueryCoordinator,
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

            using var fs = await CreateFileStreamFromFilePicker();
            if (fs is null) return;

            await csvSerializer.WriteAsync(
                fs,
                itemPageSearchState.Results,
                [
                    nameof(ItemEditModel.Id),
                    nameof(ItemEditModel.Name),
                    nameof(ItemEditModel.Price),
                    nameof(ItemEditModel.Description),
                    nameof(ItemEditModel.RarityId),
                    nameof(ItemEditModel.RowVersionBase64),
                ]);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task ExportBulkInsertBaseCsv()
        {
            using var fs = await CreateFileStreamFromFilePicker();
            if (fs is null) return;
            await csvSerializer.WriteTemplateAsync<BulkInsertItemRow>(fs);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task ExportBulkUpdateBaseCsv()
        {
            using var fs = await CreateFileStreamFromFilePicker();
            if (fs is null) return;
            await csvSerializer.WriteTemplateAsync<BulkUpdateItemRow>(fs);
        }

        private async Task<FileStream?> CreateFileStreamFromFilePicker()
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
