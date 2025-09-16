using GameTools.Server.Domain.Entities;

namespace GameTools.Server.Application.Abstractions.Stores.WriteStore
{
    public interface IRarityWriteStore : IWriteStore<Rarity, byte>
    {
        void SetOriginalRowVersion(Rarity entity, byte[] rowVersion);
    }
}
