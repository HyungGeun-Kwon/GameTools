using GameTools.Domain.Common;
using GameTools.Domain.Common.Rules;

namespace GameTools.Domain.Entities
{
    public sealed class Item : Entity<int>
    {
        public string Name { get; private set; } = default!;
        public int Price { get; private set; }
        public string? Description { get; private set; }

        // FK
        public byte RarityId { get; private set; }
        public Rarity Rarity { get; private set; } = null!;

        private Item() { } // EF Core

        public Item(string name, int price, Rarity rarity, string? description = null)
        {
            SetName(name);
            SetPrice(price);
            SetRarity(rarity);
            SetDescription(description);
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            name = name.Trim();
            if (name.Length > ItemRules.NameMax)
                throw new ArgumentException($"Name length must be <= {ItemRules.NameMax}.", nameof(name));

            Name = name;
        }

        public void SetPrice(int price)
        {
            if (price < ItemRules.PriceMin)
                throw new ArgumentOutOfRangeException(nameof(price), $"Price must be >= {ItemRules.PriceMin}.");
            Price = price;
        }

        private void SetDescription(string? description)
        {
            if (description is { Length: > ItemRules.DescriptionMax })
                throw new ArgumentException($"Description length must be <= {ItemRules.DescriptionMax}.", nameof(description));

            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }

        private void SetRarity(Rarity rarity)
        {
            ArgumentNullException.ThrowIfNull(rarity);

            Rarity = rarity;
            RarityId = rarity.Id;
        }
    }

}
