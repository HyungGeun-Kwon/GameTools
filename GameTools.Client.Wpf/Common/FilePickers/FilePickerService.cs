using Microsoft.Win32;

namespace GameTools.Client.Wpf.Common.FilePickers
{
    internal class FilePickerService : IFilePickerService
    {
        public Task<string?> OpenFileAsync(
            string title, 
            IEnumerable<FileTypeFilter> filters, 
            string? initialDirectory = null)
        {
            var dlg = new OpenFileDialog
            {
                Title = title,
                Filter = FileDialogFilters.Compose(filters.ToArray()),
                CheckFileExists = true,
                InitialDirectory = initialDirectory ?? string.Empty,
                Multiselect = false
            };
            return Task.FromResult(dlg.ShowDialog() == true ? dlg.FileName : null);
        }

        public Task<IReadOnlyList<string>> OpenFilesAsync(
            string title, 
            IEnumerable<FileTypeFilter> filters, 
            string? initialDirectory = null)
        {
            var dlg = new OpenFileDialog
            {
                Title = title,
                Filter = FileDialogFilters.Compose(filters.ToArray()),
                CheckFileExists = true,
                InitialDirectory = initialDirectory ?? string.Empty,
                Multiselect = true
            };
            return Task.FromResult(dlg.ShowDialog() == true
                ? (IReadOnlyList<string>)dlg.FileNames
                : []);
        }

        public Task<string?> SaveFileAsync(
            string title, 
            IEnumerable<FileTypeFilter> filters, 
            string defaultFileName, 
            string? defaultExtension = null, 
            string? initialDirectory = null, 
            bool overwritePrompt = true)
        {
            var filterArray = filters.ToArray();
            var dlg = new SaveFileDialog
            {
                Title = title,
                FileName = defaultFileName,
                Filter = FileDialogFilters.Compose(filterArray),
                AddExtension = true,
                DefaultExt = defaultExtension
                    ?? filterArray.FirstOrDefault(f => f.GetDefaultExtWithoutDot() is not null)?.GetDefaultExtWithoutDot(),
                InitialDirectory = initialDirectory ?? string.Empty,
                OverwritePrompt = overwritePrompt
            };
            return Task.FromResult(dlg.ShowDialog() == true ? dlg.FileName : null);
        }
    }
}
