using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Features.Rarities.Entities
{
    public sealed class Rarity
    {
        public RarityId Id { get; private set; } = null!;
        public RarityGrade Grade { get; private set; } = null!;
        public RarityColorCode ColorCode { get; private set; } = null!;

        private Rarity() { } // EF Core

        public Rarity(RarityId id, RarityGrade grade, RarityColorCode colorCode)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Grade = grade ?? throw new ArgumentNullException(nameof(grade));
            ColorCode = colorCode ?? throw new ArgumentNullException(nameof(colorCode));
        }

        public void ChangeGrade(RarityGrade newGrade)
        {
            ArgumentNullException.ThrowIfNull(newGrade);
            if (newGrade == Grade) return;
            Grade = newGrade;
        }

        public void ChangeColor(RarityColorCode newColor)
        {
            ArgumentNullException.ThrowIfNull(newColor);
            if (newColor == ColorCode) return;
            ColorCode = newColor;
        }
    }
}
