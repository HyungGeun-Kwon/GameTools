using GameTools.Server.Domain.Catalog.Rarities.Exceptions;

namespace GameTools.Server.Domain.Catalog.Rarities.ValueObjects
{
    public sealed record RarityGrade
    {
        public const int MinLength = 1;
        public const int MaxLength = 32;

        public string Value { get; }

        public RarityGrade(string value)
        {
            var s = (value ?? throw new ArgumentNullException(nameof(value))).Trim();
            if (s.Length < MinLength) throw new RarityGradeTooShortException(MinLength);
            if (s.Length > MaxLength) throw new RarityGradeTooLongException(MaxLength);

            Value = s;
        }

        public override string ToString() => Value;
    }
}
