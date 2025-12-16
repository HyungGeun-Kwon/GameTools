namespace GameTools.Server.Application.Catalog.Rarities.Commands.CreateRarity
{
    public sealed record CreateRarityResult(
        Guid Id,
        byte[] RowVersion
    );
}
