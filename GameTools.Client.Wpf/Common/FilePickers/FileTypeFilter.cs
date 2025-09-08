namespace GameTools.Client.Wpf.Common.FilePickers
{
    public sealed record FileTypeFilter(string DisplayName, params string[] Extensions)
    {
        public string ToDialogFilter()
        {
            var norms = Extensions.Select(Normalize).ToArray();
            var labelPart = string.Join("; ", norms);
            var valuePart = string.Join(";", norms);
            return $"{DisplayName} ({labelPart})|{valuePart}";
        }

        public string? GetDefaultExtWithoutDot()
        {
            var first = Extensions.FirstOrDefault(e => e != "*.*");
            if (string.IsNullOrWhiteSpace(first)) return null;
            var s = Normalize(first);
            return s.StartsWith("*.") ? s[2..] : s.TrimStart('.');
        }

        private static string Normalize(string ext)
        {
            ext = ext.Trim();
            if (ext == "*.*") return "*.*";
            if (ext.StartsWith("*.")) return ext;
            if (ext.StartsWith(".")) return $"*{ext}";
            return $"*.{ext}";
        }
    }
}
