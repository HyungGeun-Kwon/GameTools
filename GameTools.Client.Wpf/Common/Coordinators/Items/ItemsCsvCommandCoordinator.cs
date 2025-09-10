using System.ComponentModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTools.Client.Application.Ports;
using GameTools.Client.Application.UseCases.Items.BulkDeleteItems;
using GameTools.Client.Application.UseCases.Items.BulkInsertItems;
using GameTools.Client.Application.UseCases.Items.BulkUpdateItems;
using GameTools.Client.Application.UseCases.Items.GetItemsPage;
using GameTools.Client.Wpf.Common.FilePickers;
using GameTools.Client.Wpf.Common.State;
using GameTools.Client.Wpf.ViewModels.Items.Contracts;
using GameTools.Client.Wpf.ViewModels.Items.Mappers;

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
        private readonly GetItemsPageUseCase _getItemPageUseCase;
        private readonly BulkInsertItemsUseCase _bulkInsertItemsUseCase;
        private readonly BulkUpdateItemsUseCase _bulkUpdateItemsUseCase;
        private readonly BulkDeleteItemsUseCase _bulkDeleteItemsUseCase;

        public ItemsCsvCommandCoordinator(
            IItemPageSearchState itemPageSearchState,
            ICsvSerializer csvSerializer,
            IFilePickerService filePicker,
            GetItemsPageUseCase getItemPageUseCase,
            BulkInsertItemsUseCase bulkInsertItemsUseCase,
            BulkUpdateItemsUseCase bulkUpdateItemsUseCase,
            BulkDeleteItemsUseCase bulkDeleteItemsUseCase)
        {
            _itemPageSearchState = itemPageSearchState;
            _csvSerializer = csvSerializer;
            _filePicker = filePicker;
            _getItemPageUseCase = getItemPageUseCase;
            _bulkInsertItemsUseCase = bulkInsertItemsUseCase;
            _bulkUpdateItemsUseCase = bulkUpdateItemsUseCase;

            _itemPageSearchState.BusyState.PropertyChanged += OnItemPageSearchStatePropertyChanged;
            _bulkDeleteItemsUseCase = bulkDeleteItemsUseCase;
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
            var path = await PromptSaveCsvAsync("Save CSV", "Items_Export");
            if (string.IsNullOrWhiteSpace(path)) return;

            await RunExclusiveCommandAsync(async ct =>
            {
                var include = includeColumns?.ToArray() ?? _defaultInclude;

                await using var fs = File.Create(path);
                await _csvSerializer.WriteAsync(fs, items, include, ct: ct);
            }, external);
        }

        public Task ExportBulkInsertTemplateAsync(CancellationToken external = default)
              => ExportTemplate<BulkInsertItemInputRow>("Insert", external: external);

        public Task ExportBulkUpdateTemplateAsync(CancellationToken external = default)
            => ExportTemplate<BulkUpdateItemInputRow>("Update", external: external);

        public Task ExportBulkDeleteTemplateAsync(CancellationToken external = default)
            => ExportTemplate<BulkDeleteItemInputRow>("Delete", external: external);

        private async Task ExportTemplate<TInputRow>(
            string keyword, IEnumerable<string>? includeColumns = null, CancellationToken external = default)
        {
            var path = await PromptSaveCsvAsync($"Items {keyword} Template", $"Items_{keyword}_Template");
            if (string.IsNullOrWhiteSpace(path)) return;

            await RunExclusiveCommandAsync(async ct =>
            {
                var include = includeColumns?.ToArray();

                await using var fs = File.Create(path);
                await _csvSerializer.WriteTemplateAsync<TInputRow>(fs, include, ct: ct);
            }, external);
        }

        public Task ImportAndBulkInsertAsync(CancellationToken external = default)
            => RunImportAndBulkAsync<BulkInsertItemInputRow, BulkInsertItemsOutput, BulkInsertItemOutputRow>(
                "Select Insert CSV", "Insert Complete.", "Save Insert Result", "Items_Insert_Result",
                (rows, ct) => _bulkInsertItemsUseCase.Handle(new(rows), ct),
                o => o.Outputs,
                o => o.Outputs.Select(x => x.Status.ToString()),
                external);

        public Task ImportAndBulkUpdateAsync(CancellationToken external = default)
            => RunImportAndBulkAsync<BulkUpdateItemInputRow, BulkUpdateItemsOutput, BulkUpdateItemOutputRow>(
                "Select Update CSV", "Update Complete.", "Save Update Result", "Items_Update_Result",
                (rows, ct) => _bulkUpdateItemsUseCase.Handle(new(rows), ct),
                o => o.Outputs,
                o => o.Outputs.Select(x => x.Status.ToString()), external);

        public Task ImportAndBulkDeleteAsync(CancellationToken external = default)
            => RunImportAndBulkAsync<BulkDeleteItemInputRow, BulkDeleteItemsOutput, BulkDeleteItemOutputRow>(
                "Select Delete CSV", "Delete Complete.", "Save Delete Result", "Items_Delete_Result",
                (rows, ct) => _bulkDeleteItemsUseCase.Handle(new(rows), ct),
                o => o.Outputs,
                o => o.Outputs.Select(x => x.Status.ToString()), external);

        public Task ImportAndAllResultDeleteAsync(CancellationToken external = default)
        {
            return RunExclusiveCommandAsync(async ct =>
            {
                if (_itemPageSearchState.Results.Count == 0) return;


                var msgBoxBtnResult = MessageBox.Show("Would you like to save the delete results?", "", MessageBoxButton.YesNo);

                bool isSave = false;
                string? dirName = null, fileName = null, extension = ".csv";
                
                if (msgBoxBtnResult == MessageBoxResult.Yes)
                {
                    var path = await PromptSaveCsvAsync("Save All Search Result Delete Result", "Items_AllSearchResultDelete_Result");

                    if (path != null)
                    {
                        isSave = true;
                        (dirName, fileName, extension) = SplitPath(path!);
                    }
                }
                
                var totalPages = _itemPageSearchState.TotalPageNumber;
                if (totalPages <= 0) return;

                for (int page = totalPages; page >= 1; page--)
                {
                    ct.ThrowIfCancellationRequested();

                    var input = _itemPageSearchState.GetItemPageInputFromNewPage(page, 1000);
                    var pageResultOutput = await _getItemPageUseCase.Handle(input, ct);

                    var deleteOutput = await _bulkDeleteItemsUseCase.Handle(pageResultOutput.ToBulkDeleteItemsInput(), ct);

                    if (isSave && !string.IsNullOrWhiteSpace(dirName) && !string.IsNullOrWhiteSpace(fileName))
                    {
                        // 결과 저장 (페이지 단위로 파일 분리)
                        var outPath = BuildPagePath(dirName!, fileName!, extension, page);
                        await using var saveFs = File.Create(outPath);
                        await _csvSerializer.WriteAsync(saveFs, deleteOutput.Outputs, ct: ct);
                    }
                }
            }, external);
        }

        private async Task RunImportAndBulkAsync<TInRow, TOut, TWrite>(
            string openTitle,
            string successTitle,
            string saveTitle,
            string saveDefaultName,
            Func<IReadOnlyList<TInRow>, CancellationToken, Task<TOut>> handle, // UseCase 실행
            Func<TOut, IEnumerable<TWrite>> resultsSelector, // CSV로 저장할 실제 결과 컬렉션
            Func<TOut, IEnumerable<string>> statusesSelector, // 요약 메시지에 쓸 상태 문자열들
            CancellationToken external = default)
        {
            var files = await _filePicker.OpenFilesAsync(openTitle, [FileDialogFilters.Csv]);
            if (files.Count == 0) return;

            await RunExclusiveCommandAsync(async ct =>
            {
                // CSV 모두 읽기
                var rows = await ReadAllRowsAsync<TInRow>(files, ct);

                // 유스케이스 실행
                var output = await handle(rows, ct);
                var results = resultsSelector(output).ToList();
                var statuses = statusesSelector(output).ToList();

                // 요약 메시지
                string msg = BuildSummary(statuses, results.Count);

                var msgBoxBtnResult = MessageBox.Show(
                    $"{successTitle}{Environment.NewLine}" +
                    "Would you like to save the results?" + Environment.NewLine +
                    msg, "", MessageBoxButton.YesNo);

                if (msgBoxBtnResult != MessageBoxResult.Yes) return;

                // 저장
                var path = await PromptSaveCsvAsync(saveTitle, saveDefaultName);

                if (string.IsNullOrWhiteSpace(path)) return;

                await using var saveFs = File.Create(path);
                await _csvSerializer.WriteAsync(saveFs, results, ct: ct);
            }, external);
        }

        private static string BuildSummary(IEnumerable<string> statuses, int totalCount)
        {
            if (totalCount == 0) return "No items processed.";

            var parts = statuses
                .GroupBy(s => s)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .Select(g => $"{g.Key}={g.Count()}");

            return $"Total {totalCount} items: {string.Join(", ", parts)}";
        }

        private async Task<List<T>> ReadAllRowsAsync<T>(IReadOnlyList<string> files, CancellationToken ct)
        {
            var rows = new List<T>();
            foreach (var file in files)
            {
                ct.ThrowIfCancellationRequested();
                await using var fs = File.OpenRead(file);
                rows.AddRange(await _csvSerializer.ReadAsync<T>(fs, ct: ct));
            }
            return rows;
        }

        private async Task<string?> PromptSaveCsvAsync(string title, string defaultName)
        {
            var path = await _filePicker.SaveFileAsync(
                title, [FileDialogFilters.Csv, FileDialogFilters.All], defaultName, "csv");
            return string.IsNullOrWhiteSpace(path) ? null : path;
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


        private static (string? Dir, string? FileBase, string Ext) SplitPath(string path)
        {
            var dir = Path.GetDirectoryName(path);
            var ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext)) ext = ".csv";
            var name = Path.GetFileNameWithoutExtension(path);
            return (dir, name, ext);
        }
        
        private static string BuildPagePath(string dir, string fileBase, string ext, int page)
            => Path.Combine(dir, $"{fileBase}_p{page}{ext}");

        public void Dispose()
        {
            _itemPageSearchState.BusyState.PropertyChanged -= OnItemPageSearchStatePropertyChanged;
            try { _cts?.Cancel(); } catch { }
            _cts?.Dispose();
        }
    }
}
