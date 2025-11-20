using GameTools.Server.Domain.Features.Rarities.Entities;

namespace GameTools.Server.Application.Abstractions.Stores.WriteStore
{
    public interface IRarityWriteStore : IWriteStore<Rarity, Guid>;
}
