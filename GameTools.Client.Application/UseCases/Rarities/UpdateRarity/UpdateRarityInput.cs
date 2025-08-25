namespace GameTools.Client.Application.UseCases.Rarities.UpdateRarity
{
    public sealed record UpdateRarityInput(byte Id, string Grade, string ColorCode, string RowVersionBase64);
}
