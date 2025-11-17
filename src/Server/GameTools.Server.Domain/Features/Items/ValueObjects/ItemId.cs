namespace GameTools.Server.Domain.Features.Items.ValueObjects
{
    public sealed record ItemId
    {
        public Guid Value { get; }

        // 외부 임의 생성 금지
        private ItemId(Guid value) => Value = value;

        public static ItemId New() => new(Guid.NewGuid());
        public static ItemId From(Guid value)
            => value == Guid.Empty ? throw new ArgumentException("Empty Guid not allowed.", nameof(value)) : new(value);

        public static ItemId Parse(string s) => new(Guid.Parse(s));

        public static bool TryParse(string? s, out ItemId? result)
        {
            result = null;

            if (Guid.TryParse(s, out var g) && g != Guid.Empty)
            {
                result = new ItemId(g);
                return true;
            }
            return false;
        }

        public override string ToString() => Value.ToString();
    }
}
