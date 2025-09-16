namespace GameTools.Server.Application.Features.Rarities.Commands.DeleteRarity
{
    public sealed record DeleteRarityPayload(byte Id, byte[] RowVersion);
}
