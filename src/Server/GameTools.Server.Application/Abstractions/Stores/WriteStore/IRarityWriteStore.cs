using GameTools.Server.Domain.Features.Rarities.Entities;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Application.Abstractions.Stores.WriteStore
{
    public interface IRarityWriteStore : IWriteStore<Rarity, RarityId>;
}
