namespace GameTools.Client.Wpf.Shared.Services.FilePickers
{
    public interface IFilePickerService
    {
        Task<string?> OpenFileAsync(
            string title,
            IEnumerable<FileTypeFilter> filters,
            string? initialDirectory = null);

        Task<IReadOnlyList<string>> OpenFilesAsync(
            string title,
            IEnumerable<FileTypeFilter> filters,
            string? initialDirectory = null);

        Task<string?> SaveFileAsync(
            string title,
            IEnumerable<FileTypeFilter> filters,
            string defaultFileName,
            string? defaultExtension = null,
            string? initialDirectory = null,
            bool overwritePrompt = true);
    }
}
