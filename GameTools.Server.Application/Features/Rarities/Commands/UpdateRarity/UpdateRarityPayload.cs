namespace GameTools.Server.Application.Features.Rarities.Commands.UpdateRarity
{
    public sealed record UpdateRarityPayload(byte Id, string Grade, string ColorCode, byte[] RowVersion);
}
