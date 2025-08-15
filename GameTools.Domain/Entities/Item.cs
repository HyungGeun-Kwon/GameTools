using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTools.Domain.Common;

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
            if (name.Length > 100) throw new ArgumentException("Name cannot exceed 100 characters.", nameof(name));

            Name = name;
        }

        public void SetPrice(int price)
        {
            if (price < 0) throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");


            Price = price;
        }

        private void SetDescription(string? description)
        {
            if (description is { Length: > 500 }) throw new ArgumentException("Description cannot exceed 500 characters.", nameof(description));
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
