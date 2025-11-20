namespace GameTools.Server.Application.Features.Rarities.Commands.CreateRarity
{
    public sealed record CreateRarityResult(
        Guid Id,
        byte[] RowVersion
    );
}
