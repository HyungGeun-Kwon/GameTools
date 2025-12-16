namespace GameTools.Server.Domain.Catalog.Rarities.ValueObjects
{
    public sealed record RarityId
    {
        public Guid Value { get; }

        // 외부 임의 생성 금지
        private RarityId(Guid value) => Value = value;

        public static RarityId New() => new(Guid.NewGuid());
        public static RarityId From(Guid value)
            => value == Guid.Empty ? throw new ArgumentException("Empty Guid not allowed.", nameof(value)) : new(value);

        public static RarityId Parse(string s) => new(Guid.Parse(s));

        public static bool TryParse(string? s, out RarityId? result)
        {
            result = null;

            if (Guid.TryParse(s, out var g) && g != Guid.Empty)
            {
                result = new RarityId(g);
                return true;
            }
            return false;
        }

        public override string ToString() => Value.ToString();
    }
}
