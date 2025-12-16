using GameTools.Server.Domain.Catalog.Rarities.Entities;
using GameTools.Server.Domain.Catalog.Rarities.Policies;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Catalog.Rarities.Factories
{
    public class RarityFactory(
        IRarityGradeUniquenessPolicy rarityGradeUniquenessPolicy,
        IRarityColorCodeUniquenessPolicy rarityColorCodeUniquenessPolicy)
        : IRarityFactory
    {
        public async Task<Rarity> CreateAsync(
            RarityGrade grade,
            RarityColorCode colorCode,
            CancellationToken ct)
        {
            await rarityGradeUniquenessPolicy.EnsureUniqueAsync(grade, ct);
            await rarityColorCodeUniquenessPolicy.EnsureUniqueAsync(colorCode, ct);

            return new Rarity(RarityId.New(), grade, colorCode);
        }
    }
}
