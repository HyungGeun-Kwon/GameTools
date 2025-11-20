namespace GameTools.Server.Application.Features.Rarities.Commands.Common.Specs
{
    public sealed record DeleteRaritySpec(Guid Id, byte[] RowVersion);
}
