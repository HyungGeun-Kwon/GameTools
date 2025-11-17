namespace GameTools.Server.Domain.Features.Items.ValueObjects
{
    public sealed record ItemPrice
    {
        public const int MinValue = 0;

        public int Value { get; }

        public ItemPrice(int value)
        {
            if (value < MinValue) throw new ArgumentOutOfRangeException(nameof(value), $"Price must be > {MinValue}.");
            Value = value;
        }

        public override string ToString() => Value.ToString();
    }
}
