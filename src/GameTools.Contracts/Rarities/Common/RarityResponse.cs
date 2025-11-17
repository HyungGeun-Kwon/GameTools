namespace GameTools.Contracts.Rarities.Common
{
    public sealed record RarityResponse(byte Id, string Grade, string ColorCode, string RowVersionBase64);
}
