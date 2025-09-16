using System.IO;
using System.Windows;
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
    public sealed partial class ItemsCsvCommandCoordinator(
        IItemPageSearchState itemPageSearchState,
        ICsvSerializer csvSerializer,
        IFilePickerService filePicker,
        GetItemsPageUseCase getItemsPageUseCase,
        BulkInsertItemsUseCase bulkInsertItemsUseCase,
        BulkUpdateItemsUseCase bulkUpdateItemsUseCase,
        BulkDeleteItemsUseCase bulkDeleteItemsUseCase
    ) : CoordinatorBase(
            busyNotifier: itemPageSearchState.BusyState,
            busyPropertyName: nameof(itemPageSearchState.BusyState.CommandBusy),
            isBusy: () => itemPageSearchState.BusyState.CommandBusy,
            setBusy: v => itemPageSearchState.BusyState.CommandBusy = v
        ), IItemsCsvCommandCoordinator
    {
        private readonly string[] _defaultInclude = [
            nameof(ItemEditModel.Id),
            nameof(ItemEditModel.Name),
            nameof(ItemEditModel.Price),
            nameof(ItemEditModel.Description),
            nameof(ItemEditModel.RarityId),
            nameof(ItemEditModel.RowVersionBase64)
        ];

        public Task ExportPageResultsAsync(IEnumerable<ItemEditModel> items, IEnumerable<string>? includeColumns = null, CancellationToken external = default)
            => RunExclusiveAsync(async ct =>
            {
                var path = await PromptSaveCsvAsync("Save CSV", "Items_Export");
                if (string.IsNullOrWhiteSpace(path)) return;

                var include = includeColumns?.ToArray() ?? _defaultInclude;
                await using var fs = File.Create(path);
                await csvSerializer.WriteAsync(fs, items, include, ct: ct);
            }, external);

        public Task ExportBulkInsertTemplateAsync(CancellationToken external = default)
            => ExportTemplate<BulkInsertItemInputRow>("Insert", external: external);

        public Task ExportBulkUpdateTemplateAsync(CancellationToken external = default)
            => ExportTemplate<BulkUpdateItemInputRow>("Update", external: external);

        public Task ExportBulkDeleteTemplateAsync(CancellationToken external = default)
            => ExportTemplate<BulkDeleteItemInputRow>("Delete", external:external);

        private Task ExportTemplate<TInputRow>(string keyword, IEnumerable<string>? includeColumns = null, CancellationToken external = default)
            => RunExclusiveAsync(async ct =>
            {
                var path = await PromptSaveCsvAsync($"Items {keyword} Template", $"Items_{keyword}_Template");
                if (string.IsNullOrWhiteSpace(path)) return;

                var include = includeColumns?.ToArray();
                await using var fs = File.Create(path);
                await csvSerializer.WriteTemplateAsync<TInputRow>(fs, include, ct: ct);
            }, external);

        public Task ImportAndBulkInsertAsync(CancellationToken external = default)
            => RunImportAndBulkAsync<BulkInsertItemInputRow, BulkInsertItemsOutput, BulkInsertItemOutputRow>(
                "Select Insert CSV", "Insert Complete.", "Save Insert Result", "Items_Insert_Result",
                (rows, ct) => bulkInsertItemsUseCase.Handle(new(rows), ct),
                o => o.Outputs,
                o => o.Outputs.Select(x => x.Status.ToString()),
                external);

        public Task ImportAndBulkUpdateAsync(CancellationToken external = default)
            => RunImportAndBulkAsync<BulkUpdateItemInputRow, BulkUpdateItemsOutput, BulkUpdateItemOutputRow>(
                "Select Update CSV", "Update Complete.", "Save Update Result", "Items_Update_Result",
                (rows, ct) => bulkUpdateItemsUseCase.Handle(new(rows), ct),
                o => o.Outputs,
                o => o.Outputs.Select(x => x.Status.ToString()),
                external);

        public Task ImportAndBulkDeleteAsync(CancellationToken external = default)
            => RunImportAndBulkAsync<BulkDeleteItemInputRow, BulkDeleteItemsOutput, BulkDeleteItemOutputRow>(
                "Select Delete CSV", "Delete Complete.", "Save Delete Result", "Items_Delete_Result",
                (rows, ct) => bulkDeleteItemsUseCase.Handle(new(rows), ct),
                o => o.Outputs,
                o => o.Outputs.Select(x => x.Status.ToString()),
                external);

        public Task ImportAndAllResultDeleteAsync(CancellationToken external = default)
            => RunExclusiveAsync(async ct =>
            {
                if (itemPageSearchState.Results.Count == 0) return;

                var msgBoxBtnResult = MessageBox.Show("Would you like to save the delete results?", "", MessageBoxButton.YesNo);

                bool isSave = false;
                string? dirName = null, fileName = null, extension = ".csv";

                if (msgBoxBtnResult == MessageBoxResult.Yes)
                {
                    var path = await PromptSaveCsvAsync("Save All Search Result Delete Result", "Items_AllSearchResultDelete_Result");
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        isSave = true;
                        (dirName, fileName, extension) = SplitPath(path!);
                    }
                }

                var totalPages = itemPageSearchState.TotalPageNumber;
                if (totalPages <= 0) return;

                for (int page = totalPages; page >= 1; page--)
                {
                    ct.ThrowIfCancellationRequested();

                    var input = itemPageSearchState.GetItemPageInputFromNewPage(page, 1000);
                    var pageResultOutput = await getItemsPageUseCase.Handle(input, ct);

                    var deleteOutput = await bulkDeleteItemsUseCase.Handle(pageResultOutput.ToBulkDeleteItemsInput(), ct);

                    if (isSave && !string.IsNullOrWhiteSpace(dirName) && !string.IsNullOrWhiteSpace(fileName))
                    {
                        var outPath = BuildPagePath(dirName!, fileName!, extension, page);
                        await using var saveFs = File.Create(outPath);
                        await csvSerializer.WriteAsync(saveFs, deleteOutput.Outputs, ct: ct);
                    }
                }
            }, external);

        private async Task<string?> PromptSaveCsvAsync(string title, string defaultName)
            => await filePicker.SaveFileAsync(title, [FileDialogFilters.Csv, FileDialogFilters.All], defaultName, "csv");

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

        private async Task<List<T>> ReadAllRowsAsync<T>(IReadOnlyList<string> files, CancellationToken ct)
        {
            var rows = new List<T>();
            foreach (var file in files)
            {
                ct.ThrowIfCancellationRequested();
                await using var fs = File.OpenRead(file);
                rows.AddRange(await csvSerializer.ReadAsync<T>(fs, ct: ct));
            }
            return rows;
        }

        private Task RunImportAndBulkAsync<TInRow, TOut, TWrite>(
            string openTitle,
            string successTitle,
            string saveTitle,
            string saveDefaultName,
            Func<IReadOnlyList<TInRow>, CancellationToken, Task<TOut>> handle,
            Func<TOut, IEnumerable<TWrite>> resultsSelector,
            Func<TOut, IEnumerable<string>> statusesSelector,
            CancellationToken external = default)
            => RunExclusiveAsync(async ct =>
            {
                var files = await filePicker.OpenFilesAsync(openTitle, [FileDialogFilters.Csv]);
                if (files.Count == 0) return;

                var rows = await ReadAllRowsAsync<TInRow>(files, ct);
                var output = await handle(rows, ct);
                var results = resultsSelector(output).ToList();
                var statuses = statusesSelector(output).ToList();

                string msg = BuildSummary(statuses, results.Count);

                var msgBoxBtnResult = MessageBox.Show(
                    $"{successTitle}{Environment.NewLine}" +
                    "Would you like to save the results?" + Environment.NewLine +
                    msg, "", MessageBoxButton.YesNo);

                if (msgBoxBtnResult != MessageBoxResult.Yes) return;

                var path = await PromptSaveCsvAsync(saveTitle, saveDefaultName);
                if (string.IsNullOrWhiteSpace(path)) return;

                await using var saveFs = File.Create(path);
                await csvSerializer.WriteAsync(saveFs, results, ct: ct);
            }, external);

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
    }
}
