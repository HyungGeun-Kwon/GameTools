namespace GameTools.Server.Application.Catalog.Rarities.Commands.Common.Specs
{
    public sealed record UpdateRaritySpec(Guid Id, string Grade, string ColorCode, byte[] RowVersion)
    {
        public string NormalizedColorCode => (ColorCode ?? "").Trim().ToUpperInvariant();
    }
}
