using GameTools.Server.Domain.Catalog.Items.Exceptions;

namespace GameTools.Server.Domain.Catalog.Items.ValueObjects
{
    public sealed record ItemPrice
    {
        public const int MinValue = 0;

        public int Value { get; }

        public ItemPrice(int value)
        {
            if (value < MinValue) throw new ItemPriceTooSmallException(MinValue);
            Value = value;
        }

        public override string ToString() => Value.ToString();
    }
}
