using GameTools.Server.Domain.Catalog.Rarities.Exceptions;
using GameTools.Server.Domain.Catalog.Rarities.Services;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Catalog.Rarities.Policies
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
