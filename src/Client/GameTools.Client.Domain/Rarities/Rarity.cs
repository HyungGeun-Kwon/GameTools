namespace GameTools.Client.Domain.Rarities
{
    public sealed record Rarity(byte Id, string Grade, string ColorCode, string RowVersionBase64);
}
