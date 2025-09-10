using System.Text.RegularExpressions;

namespace GameTools.Client.Domain.Common.Rules
{
    public static class RarityRules
    {
        public const int GradeMax = 32;
        public const string DefaultColor = "#A0A0A0";
        public const string ColorFormat = "^#[0-9A-F]{6}$";

        public static string NormalizeColor(string color) => color.Trim().ToUpperInvariant();
    }
}
