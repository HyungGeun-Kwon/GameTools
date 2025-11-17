using GameTools.Server.Domain.Features.Items.Exceptions;

namespace GameTools.Server.Domain.Features.Items.ValueObjects
{
    public sealed record ItemName
    {
        public const int MinLength = 3;
        public const int MaxLength = 100;
        public string Value { get; }

        public ItemName(string value)
        {
            var s = (value ?? throw new ArgumentNullException(nameof(value))).Trim();
            if (s.Length < MinLength) throw new ItemNameTooShortException(MinLength);
            if (s.Length > MaxLength) throw new ItemNameTooLongException(MaxLength);

            Value = s;
        }

        public override string ToString() => Value;
    }
}
