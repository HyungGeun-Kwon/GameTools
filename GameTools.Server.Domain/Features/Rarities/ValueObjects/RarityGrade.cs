namespace GameTools.Server.Domain.Features.Rarities.ValueObjects
{
    public sealed record RarityGrade
    {
        public const int MaxLength = 32;

        public string Value { get; }

        public RarityGrade(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Rarity grade cannot be null or whitespace.", nameof(value));

            var s = value.Trim();
            if (s.Length > MaxLength) // Assuming a max length rule
                throw new ArgumentException($"Rarity grade is too long (max {MaxLength}).", nameof(value));


            Value = s;
        }

        public static RarityGrade Parse(string s) => new(s);

        public static bool TryParse(string s, out RarityGrade? result)
        {
            result = null;
            try
            {
                result = new(s);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public override string ToString() => Value;
    }
}
