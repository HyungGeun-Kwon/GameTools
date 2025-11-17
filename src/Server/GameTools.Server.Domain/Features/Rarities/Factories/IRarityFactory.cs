using GameTools.Server.Domain.Features.Rarities.Entities;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Features.Rarities.Factories
{
    public interface IRarityFactory
    {
        Task<Rarity> CreateAsync(
            RarityGrade grade,
            RarityColorCode colorCode,
            CancellationToken ct);
    }
}