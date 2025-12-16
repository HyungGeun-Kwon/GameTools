using GameTools.Server.Domain.Catalog.Rarities.Entities;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;

namespace GameTools.Server.Application.Abstractions.Stores.WriteStore
{
    public interface IRarityWriteStore : IWriteStore<Rarity, RarityId>;
}
