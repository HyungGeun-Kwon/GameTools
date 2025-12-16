using System.Text.RegularExpressions;

namespace GameTools.Server.Domain.Catalog.Rarities.ValueObjects
{
    public sealed partial record RarityColorCode
    {
        public const int Length = 7; // e.g., "#RRGGBB"
        public string Value { get; }
        public RarityColorCode(string value)
        {
            var s = (value ?? throw new ArgumentNullException(nameof(value))).Trim().ToUpper();
            if (s.Length != Length || !HexColorRegex().IsMatch(s))
                throw new ArgumentException("Rarity color code must be in the format '#RRGGBB' (uppercase).", nameof(value));
            Value = s;
        }

        [GeneratedRegex("^#[0-9A-F]{6}$", RegexOptions.CultureInvariant)]
        public static partial Regex HexColorRegex();

        public override string ToString() => Value;
    }
}
