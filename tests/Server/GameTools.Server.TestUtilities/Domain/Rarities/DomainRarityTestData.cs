using GameTools.Server.Domain.Features.Rarities.Entities;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.TestUtilities.Domain.Rarities
{
    public static class DomainRarityTestData
    {
        public static string ValidRarityGradeValue(char c = 'r')
            => new(c, RarityGrade.MinLength);

        public static RarityGrade ValidRarityGrade(char c = 'r')
            => new(ValidRarityGradeValue(c));

        public static string ValidRarityColorCodeValue()
            => "#FFFFFF";

        public static RarityColorCode ValidRarityColorCode(string? value = null)
            => new(value ?? ValidRarityColorCodeValue());

        public static RarityId ValidRarityId(Guid? guid = null)
            => guid is null ? RarityId.New() : RarityId.From(guid.Value);

        public static Rarity BuildRarity(
            RarityId? id = null,
            RarityGrade? grade = null,
            RarityColorCode? colorCode = null)
            => new(
                id ?? ValidRarityId(),
                grade ?? ValidRarityGrade(),
                colorCode ?? ValidRarityColorCode());
    }
}
