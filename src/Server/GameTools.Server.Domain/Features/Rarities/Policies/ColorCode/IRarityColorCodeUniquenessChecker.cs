using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Features.Rarities.Policies.ColorCode
{
    public interface IRarityColorCodeUniquenessChecker
    {
        Task<bool> ExistsAsync(RarityColorCode colorCode, CancellationToken ct);
    }
}
