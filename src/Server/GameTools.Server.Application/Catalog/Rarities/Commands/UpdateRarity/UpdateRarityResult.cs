namespace GameTools.Server.Application.Catalog.Rarities.Commands.UpdateRarity
{
    public sealed record UpdateRarityResult(
        Guid Id,
        byte[] RowVersion);
}
