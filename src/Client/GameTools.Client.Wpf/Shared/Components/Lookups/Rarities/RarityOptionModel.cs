namespace GameTools.Client.Wpf.Shared.Components.Lookups.Rarities
{
    public sealed record RarityOptionModel(byte? Id, string Grade, string? ColorCode)
    {
        public override string ToString() => Grade;
        public static RarityOptionModel All() => new(null, "All", null);
    }
}
