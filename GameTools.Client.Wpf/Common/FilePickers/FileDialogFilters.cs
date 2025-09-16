namespace GameTools.Client.Wpf.Common.FilePickers
{
    public static class FileDialogFilters
    {
        public static readonly FileTypeFilter Csv = new("CSV files", "*.csv");
        public static readonly FileTypeFilter Json = new("JSON files", "*.json");
        public static readonly FileTypeFilter Excel = new("Excel files", "*.xlsx", "*.xls");
        public static readonly FileTypeFilter All = new("All files", "*.*");

        public static string Compose(params FileTypeFilter[] filters)
            => string.Join("|", filters.Select(f => f.ToDialogFilter()));
    }
}
