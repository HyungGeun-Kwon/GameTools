using System.Text.RegularExpressions;

namespace GameTools.Server.Domain.Features.Rarities.ValueObjects
{
    public sealed partial record RarityColorCode
    {
        public const int Length = 7; // e.g., "#RRGGBB"
        public string Value { get; }
        public RarityColorCode(string value)
        {
            var s = value?.Trim();
            ArgumentException.ThrowIfNullOrWhiteSpace(s, nameof(value));
            if (s.Length != Length || !HexColorRegex().IsMatch(s))
                throw new ArgumentException("Rarity color code must be in the format '#RRGGBB' (uppercase).", nameof(value));
            Value = s;
        }

        [GeneratedRegex("^#[0-9A-F]{6}$", RegexOptions.CultureInvariant)]
        private static partial Regex HexColorRegex();

        public override string ToString() => Value;
    }
}
