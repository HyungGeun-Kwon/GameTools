using GameTools.Domain.Entities;

namespace GameTools.Application.Abstractions.WriteStore
{
    public interface IRarityWriteStore : IWriteStore<Rarity, byte>
    {
        void SetOriginalRowVersion(Rarity entity, string base64);
    }
}
