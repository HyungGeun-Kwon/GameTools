namespace GameTools.Server.Application.Catalog.Rarities.Commands.Common.Specs
{
    public sealed record DeleteRaritySpec(Guid Id, byte[] RowVersion);
}
