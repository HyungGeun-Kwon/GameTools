namespace GameTools.Contracts.Rarities.DeleteRarity
{
    public sealed record DeleteRarityRequest(byte Id, string RowVersionBase64);
}
