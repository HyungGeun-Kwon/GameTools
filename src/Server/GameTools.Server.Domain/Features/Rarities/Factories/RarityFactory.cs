using GameTools.Server.Domain.Features.Rarities.Entities;
using GameTools.Server.Domain.Features.Rarities.Policies.ColorCode;
using GameTools.Server.Domain.Features.Rarities.Policies.Grade;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Features.Rarities.Factories
{
    public class RarityFactory(
        RarityGradeUniquenessPolicy rarityGradeUniquenessPolicy,
        RarityColorCodeUniquenessPolicy rarityColorCodeUniquenessPolicy)
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
