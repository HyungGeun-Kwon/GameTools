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
            if (s.Length < MinLength) throw new ArgumentException($"Item name is too short (min {MinLength}).", nameof(value));
            if (s.Length > MaxLength) throw new ArgumentException($"Item name is too long (max {MaxLength}).", nameof(value));

            Value = s;
        }

        public override string ToString() => Value;
    }
}
