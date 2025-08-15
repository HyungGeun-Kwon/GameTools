using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GameTools.Domain.Common;

namespace GameTools.Domain.Entities
{
    public sealed class Rarity : Entity<byte>
    {
        private readonly List<Item> _items = [];

        public string Grade { get; private set; } = default!;
        public string ColorCode { get; private set; } = default!;

        public IReadOnlyCollection<Item> Items => _items.AsReadOnly();

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

            Grade = grade;
        }

        public void SetColorCode(string colorCode)
        {
            if (string.IsNullOrWhiteSpace(colorCode))
                throw new ArgumentException("Color code cannot be null or empty.", nameof(colorCode));

            colorCode = colorCode.Trim().ToUpperInvariant();
            if (!Regex.IsMatch(colorCode, "^#[0-9A-F]{6}$")) throw new ArgumentException("Color code must be '#RRGGBB'.", nameof(colorCode));

            ColorCode = colorCode;
        }
    }

}
