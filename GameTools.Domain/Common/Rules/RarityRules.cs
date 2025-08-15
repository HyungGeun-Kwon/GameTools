using System.Text.RegularExpressions;

namespace GameTools.Domain.Common.Rules
{
    public static class RarityRules
    {
        public const int GradeMax = 32;
        public const string DefaultColor = "#A0A0A0";
        public static readonly Regex ColorHexRegex = new("^#[0-9A-F]{6}$", RegexOptions.Compiled);

        public static string NormalizeColor(string color) => color.Trim().ToUpperInvariant();
    }
}
