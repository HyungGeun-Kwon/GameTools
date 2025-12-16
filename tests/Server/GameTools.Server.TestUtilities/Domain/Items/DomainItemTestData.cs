using GameTools.Server.Domain.Catalog.Items.Entities;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;

namespace GameTools.Server.TestUtilities.Domain.Items
{
    public static class DomainItemTestData
    {
        public static string ValidItemNameValue(char c = 'a')
            => new(c, ItemName.MinLength);

        public static ItemName ValidItemName(char c = 'a')
            => new(ValidItemNameValue(c));

        public static string ValidItemDescriptionValue(char c = 'd')
            => new(c, Math.Min(10, ItemDescription.MaxLength));

        public static ItemDescription ValidItemDescription(char c = 'd')
            => new(ValidItemDescriptionValue(c));

        public static int ValidItemPriceValue(int? value = null)
            => value ?? ItemPrice.MinValue;

        public static ItemPrice ValidItemPrice(int? value = null)
            => new(ValidItemPriceValue(value));

        public static RarityId ValidRarityId(Guid? guid = null)
            => RarityId.From(guid ?? Guid.NewGuid());

        public static ItemId ValidItemId(Guid? guid = null)
            => guid is null ? ItemId.New() : ItemId.From(guid.Value);

        public static Item BuildItem(
            ItemId? id = null,
            ItemName? name = null,
            ItemPrice? price = null,
            ItemDescription? description = null,
            RarityId? rarityId = null)
            => new(
                id ?? ValidItemId(),
                name ?? ValidItemName(),
                price ?? ValidItemPrice(),
                description ?? ValidItemDescription(),
                rarityId ?? ValidRarityId());
    }
}
