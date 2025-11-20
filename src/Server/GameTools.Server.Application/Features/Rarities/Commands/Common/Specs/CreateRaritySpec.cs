namespace GameTools.Server.Application.Features.Rarities.Commands.Common.Specs
{
    public sealed record CreateRaritySpec(string Grade, string ColorCode)
    {
        public string NormalizedColorCode => (ColorCode ?? "").Trim().ToUpperInvariant();
    }
}
