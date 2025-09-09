using System.ComponentModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.Ports;
using GameTools.Client.Application.UseCases.Items.BulkInsertItems;
using GameTools.Client.Application.UseCases.Items.BulkUpdateItems;
using GameTools.Client.Wpf.Common.FilePickers;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Items.Contracts;

namespace GameTools.Client.Wpf.Common.Coordinators.Items
{
    public sealed partial class ItemsCsvCommandCoordinator : ObservableObject, IItemsCsvCommandCoordinator
    {
        private CancellationTokenSource? _cts;

        private readonly string[] _defaultInclude = [
            nameof(ItemEditModel.Id),
            nameof(ItemEditModel.Name),
            nameof(ItemEditModel.Price),
            nameof(ItemEditModel.Description),
            nameof(ItemEditModel.RarityId),
            nameof(ItemEditModel.RowVersionBase64)
            ];

        private readonly IItemPageSearchState _itemPageSearchState;
        private readonly ICsvSerializer _csvSerializer;
        private readonly IFilePickerService _filePicker;
        private readonly BulkInsertItemsUseCase _bulkInsertItemsUseCase;
        private readonly BulkUpdateItemsUseCase _bulkUpdateItemsUseCase;

        public ItemsCsvCommandCoordinator(
            IItemPageSearchState itemPageSearchState,
            ICsvSerializer csvSerializer,
            IFilePickerService filePicker,
            BulkInsertItemsUseCase bulkInsertItemsUseCase,
            BulkUpdateItemsUseCase bulkUpdateItemsUseCase)
        {
            _itemPageSearchState = itemPageSearchState;
            _csvSerializer = csvSerializer;
            _filePicker = filePicker;
            _bulkInsertItemsUseCase = bulkInsertItemsUseCase;
            _bulkUpdateItemsUseCase = bulkUpdateItemsUseCase;

            _itemPageSearchState.BusyState.PropertyChanged += OnItemPageSearchStatePropertyChanged;
        }
        private void OnItemPageSearchStatePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_itemPageSearchState.BusyState.CommandBusy)) CancelCommand.NotifyCanExecuteChanged();
        }

        private bool CanCancel() => _itemPageSearchState.BusyState.CommandBusy;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel() => _cts?.Cancel();

        private CancellationToken NewToken(CancellationToken external)
        {
            try { _cts?.Cancel(); } catch (ObjectDisposedException) { }
            _cts = CancellationTokenSource.CreateLinkedTokenSource(external);
            return _cts.Token;
        }

        public async Task ExportPageResultsAsync(IEnumerable<ItemEditModel> items, IEnumerable<string>? includeColumns = null, CancellationToken external = default)
        {
            var path = await _filePicker.SaveFileAsync(
                "Save CSV", [FileDialogFilters.Csv, FileDialogFilters.All], "Items_Export", "csv");
            if (string.IsNullOrWhiteSpace(path)) return;

            await RunExclusiveCommandAsync(async ct =>
            {
                var include = includeColumns?.ToArray() ?? _defaultInclude;

                await using var fs = File.Create(path);
                await _csvSerializer.WriteAsync(fs, items, include, ct: ct);
            }, external);
        }

        public async Task ExportBulkInsertTemplateAsync(CancellationToken external = default)
        {
            var path = await _filePicker.SaveFileAsync(
                "Save Insert Template", [FileDialogFilters.Csv, FileDialogFilters.All], "Items_Insert_Template", "csv");
            if (string.IsNullOrWhiteSpace(path)) return;

            await RunExclusiveCommandAsync(async ct =>
            {
                await using var fs = File.Create(path);
                await _csvSerializer.WriteTemplateAsync<BulkInsertItemInputRow>(fs, ct: ct);
            }, external);
        }

        public async Task ExportBulkUpdateTemplateAsync(CancellationToken external = default)
        {
            var path = await _filePicker.SaveFileAsync(
                "Save Update Template", [FileDialogFilters.Csv, FileDialogFilters.All], "Items_Update_Template", "csv");
            if (string.IsNullOrWhiteSpace(path)) return;

            await RunExclusiveCommandAsync(async ct =>
            {
                await using var fs = File.Create(path);
                await _csvSerializer.WriteTemplateAsync<BulkUpdateItemInputRow>(fs, ct: ct);
            }, external);
        }

        public async Task ImportAndBulkInsertAsync(CancellationToken external = default)
        {
            var files = await _filePicker.OpenFilesAsync("Select Insert CSV", [FileDialogFilters.Csv]);
            if (files.Count == 0) return;

            await RunExclusiveCommandAsync(async ct =>
            {
                var rows = new List<BulkInsertItemInputRow>();
                foreach (var file in files)
                {
                    ct.ThrowIfCancellationRequested();
                    await using var fs = File.OpenRead(file);
                    rows.AddRange(await _csvSerializer.ReadAsync<BulkInsertItemInputRow>(fs, ct: ct));
                }
                var output = await _bulkInsertItemsUseCase.Handle(new(rows), ct);

                string msg = output.Outputs.Count == 0
                    ? "No items processed."
                    : $"Total {output.Outputs.Count} items: " +
                      string.Join(", ",
                          output.Outputs
                                .GroupBy(o => o.Status)
                                .OrderByDescending(g => g.Count())
                                .ThenBy(g => g.Key)
                                .Select(g => $"{g.Key}={g.Count()}"));

                var msgBoxBtnResult = MessageBox.Show("Insert Compltete." + Environment.NewLine +
                    "Would you like to save the results?" + Environment.NewLine +
                    msg, "", MessageBoxButton.YesNo);

                if (msgBoxBtnResult != MessageBoxResult.Yes) return;

                var path = await _filePicker.SaveFileAsync(
                    "Save Insert Result", [FileDialogFilters.Csv, FileDialogFilters.All], "Items_Insert_Result", "csv");

                if (string.IsNullOrWhiteSpace(path)) return;

                await using var saveFs = File.Create(path);
                await _csvSerializer.WriteAsync(saveFs, output.Outputs, ct: ct);
            }, external);
        }

        public async Task ImportAndBulkUpdateAsync(CancellationToken external = default)
        {
            var files = await _filePicker.OpenFilesAsync("Select Update CSV", [FileDialogFilters.Csv]);
            if (files.Count == 0) return;

            await RunExclusiveCommandAsync(async ct =>
            {
                var rows = new List<BulkUpdateItemInputRow>();
                foreach (var file in files)
                {
                    ct.ThrowIfCancellationRequested();
                    await using var fs = File.OpenRead(file);
                    rows.AddRange(await _csvSerializer.ReadAsync<BulkUpdateItemInputRow>(fs, ct: ct));
                }

                BulkUpdateItemsOutput output = await _bulkUpdateItemsUseCase.Handle(new(rows), ct);

                string msg = output.Outputs.Count == 0
                    ? "No items processed."
                    : $"Total {output.Outputs.Count} items: " +
                      string.Join(", ",
                          output.Outputs
                                .GroupBy(o => o.Status)
                                .OrderByDescending(g => g.Count())
                                .ThenBy(g => g.Key)
                                .Select(g => $"{g.Key}={g.Count()}"));

                var msgBoxBtnResult = MessageBox.Show("Update Compltete." + Environment.NewLine +
                    "Would you like to save the results?" + Environment.NewLine +
                    msg, "", MessageBoxButton.YesNo);

                if (msgBoxBtnResult != MessageBoxResult.Yes) return;

                var path = await _filePicker.SaveFileAsync(
                    "Save Update Result", [FileDialogFilters.Csv, FileDialogFilters.All], "Items_Update_Result", "csv");

                if (string.IsNullOrWhiteSpace(path)) return;

                await using var saveFs = File.Create(path);
                await _csvSerializer.WriteAsync(saveFs, output.Outputs, ct: ct);
            }, external);
        }

        private async Task RunExclusiveCommandAsync(
            Func<CancellationToken, Task> action,
            CancellationToken external = default)
        {
            var token = NewToken(external);
            var myCts = _cts;

            try
            {
                SetCommandBusy(true);
                await action(token);
            }
            finally
            {
                if (ReferenceEquals(myCts, _cts))
                {
                    SetCommandBusy(false);
                    _cts = null;
                }
                myCts?.Dispose();
            }
        }

        private async Task<TResult> RunExclusiveCommandAsync<TResult>(
            Func<CancellationToken, Task<TResult>> action,
            CancellationToken external = default)
        {
            var token = NewToken(external);
            var myCts = _cts;

            try
            {
                SetCommandBusy(true);
                return await action(token);
            }
            finally
            {
                if (ReferenceEquals(myCts, _cts))
                {
                    _cts = null;
                    SetCommandBusy(false);
                }
                myCts?.Dispose();
            }
        }

        private void SetCommandBusy(bool value)
        {
            _itemPageSearchState.BusyState.CommandBusy = value;
            CancelCommand.NotifyCanExecuteChanged();
        }


        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
