using GameTools.Server.Domain.Features.Items.ValueObjects;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Features.Items.Entities
{
    public sealed class Item
    {
        public ItemId Id { get; private set; } = null!;
        public ItemName Name { get; private set; } = null!;
        public ItemPrice Price { get; private set; } = null!;
        public ItemDescription Description { get; private set; } = null!;

        public RarityId RarityId { get; private set; } = null!;

        private Item() { } // EF Core

        internal Item(ItemId id, ItemName name, ItemPrice price, ItemDescription description, RarityId rarityId)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Price = price ?? throw new ArgumentNullException(nameof(price));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            RarityId = rarityId ?? throw new ArgumentNullException(nameof(rarityId));
        }

        public void Rename(ItemName newName)
        {
            ArgumentNullException.ThrowIfNull(newName);
            if (newName == Name) return;
            Name = newName;
        }

        public void ChangePrice(ItemPrice newPrice)
        {
            ArgumentNullException.ThrowIfNull(newPrice);
            if (newPrice == Price) return;
            Price = newPrice;

            // _domainEvents.Add(new ItemPriceChanged(Id, old: Price, newPrice));
        }

        public void ChangeDescription(ItemDescription newDescription)
        {
            ArgumentNullException.ThrowIfNull(newDescription);
            if (newDescription == Description) return;
            Description = newDescription;
        }

        public void ChangeRarity(RarityId newRarityId)
        {
            ArgumentNullException.ThrowIfNull(newRarityId);
            if (newRarityId == RarityId) return;
            RarityId = newRarityId;
        }
    }
}
