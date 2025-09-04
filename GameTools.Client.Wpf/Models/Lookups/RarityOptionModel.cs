namespace GameTools.Client.Wpf.Models.Lookups
{
    public sealed record RarityOptionModel(byte? Id, string Grade)
    {
        public override string ToString() => Grade;
        public static RarityOptionModel All() => new(null, "All");
    }
}
