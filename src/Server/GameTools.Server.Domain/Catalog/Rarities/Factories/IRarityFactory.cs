using GameTools.Server.Domain.Catalog.Rarities.Entities;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Catalog.Rarities.Factories
{
    public interface IRarityFactory
    {
        Task<Rarity> CreateAsync(
            RarityGrade grade,
            RarityColorCode colorCode,
            CancellationToken ct);
    }
}