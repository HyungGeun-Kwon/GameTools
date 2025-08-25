namespace GameTools.Client.Application.UseCases.Rarities.DeleteRarity
{
    public sealed record DeleteRarityInput(byte Id, string RowVersionBase64);
}
