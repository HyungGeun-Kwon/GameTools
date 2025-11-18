using GameTools.Server.Domain.Features.Rarities.Exceptions;
using GameTools.Server.Domain.Features.Rarities.Services;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Features.Rarities.Policies
{
    public sealed class RarityColorCodeUniquenessPolicy(IRarityColorCodeUniquenessChecker checker) : IRarityColorCodeUniquenessPolicy
    {
        public async Task EnsureUniqueAsync(RarityColorCode colorCode, CancellationToken ct)
        {
            if (await checker.ExistsAsync(colorCode, ct))
                throw new RarityColorCodeNotUniqueException(colorCode);
        }
    }
}
