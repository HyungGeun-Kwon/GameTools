using GameTools.Server.Domain.Features.Items.Entities;
using GameTools.Server.Domain.Features.Items.ValueObjects;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Features.Items.Factories
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
