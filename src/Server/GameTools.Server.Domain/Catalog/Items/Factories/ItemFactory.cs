using GameTools.Server.Domain.Catalog.Items.Entities;
using GameTools.Server.Domain.Catalog.Items.Policies;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Catalog.Items.Factories
{
    public sealed class ItemFactory(IItemNameUniquenessPolicy itemNameUniqunessPolicy) : IItemFactory
    {
        public async Task<Item> CreateAsync(ItemName name, ItemPrice price, ItemDescription description, RarityId rarityId, CancellationToken ct)
        {
            await itemNameUniqunessPolicy.EnsureUniqueAsync(name, ct);

            return new Item(
                ItemId.New(),
                name,
                price,
                description,
                rarityId);
        }
    }
}
