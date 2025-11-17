using GameTools.Contracts.Rarities.Common;

namespace GameTools.Contracts.Rarities.GetAllRarities
{
    public sealed record AllRarityResponse(IReadOnlyList<RarityResponse> Rarities);
}
