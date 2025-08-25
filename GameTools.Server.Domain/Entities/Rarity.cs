using System.ComponentModel.DataAnnotations;
using GameTools.Server.Domain.Common;
using GameTools.Server.Domain.Common.Rules;

namespace GameTools.Server.Domain.Entities
{
    public sealed class Rarity : Entity<byte>
    {
        private readonly List<Item> _items = [];

        public string Grade { get; private set; } = default!;
        public string ColorCode { get; private set; } = default!;

        public IReadOnlyCollection<Item> Items => _items.AsReadOnly();

        [Timestamp]
        public byte[] RowVersion { get; private set; } = default!;

        private Rarity() { } // EF Core

        public Rarity(string grade, string colorCode)
        {
            SetGrade(grade);
            SetColorCode(colorCode);
        }

        public void SetGrade(string grade)
        {
            if (string.IsNullOrWhiteSpace(grade))
                throw new ArgumentException("Grade cannot be null or empty.", nameof(grade));

            grade = grade.Trim();
            if (grade.Length > RarityRules.GradeMax)
                throw new ArgumentException($"Grade length must be <= {RarityRules.GradeMax}.", nameof(grade));

            Grade = grade;
        }

        public void SetColorCode(string colorCode)
        {
            if (string.IsNullOrWhiteSpace(colorCode))
                throw new ArgumentException("Color code cannot be null or empty.", nameof(colorCode));

            colorCode = RarityRules.NormalizeColor(colorCode);
            if (!RarityRules.ColorHexRegex.IsMatch(colorCode))
                throw new ArgumentException("Color must be '#RRGGBB' (uppercase).", nameof(colorCode));

            ColorCode = colorCode;
        }
    }

}
