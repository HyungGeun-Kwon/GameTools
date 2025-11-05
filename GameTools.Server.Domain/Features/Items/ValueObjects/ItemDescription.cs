namespace GameTools.Server.Domain.Features.Items.ValueObjects
{
    public sealed record ItemDescription
    {
        public const int MaxLength = 1000;

        public string? Value { get; }

        public ItemDescription(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Value = null;
                return;
            }

            var s = value.Trim();
            if (s.Length > MaxLength)
                throw new ArgumentException($"Item Description is too long (max {MaxLength}).", nameof(value));

            Value = s;
        }

        public override string ToString() => Value ?? string.Empty;
    }
}
