using GameTools.Server.Domain.Catalog.Items.Entities;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Catalog.Items.Factories
{
    public interface IItemFactory
    {
        Task<Item> CreateAsync(
            ItemName name,
            ItemPrice price,
            ItemDescription description,
            RarityId rarityId,
            CancellationToken ct);
    }
}
