namespace GameTools.Contracts.Rarities.UpdateRarity
{
    public sealed record UpdateRarityRequest(byte Id, string Grade, string ColorCode, string RowVersionBase64);
}
