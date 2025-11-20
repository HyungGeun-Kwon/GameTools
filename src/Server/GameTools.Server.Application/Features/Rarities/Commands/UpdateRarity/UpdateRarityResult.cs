namespace GameTools.Server.Application.Features.Rarities.Commands.UpdateRarity
{
    public sealed record UpdateRarityResult(
        Guid Id,
        byte[] RowVersion);
}
