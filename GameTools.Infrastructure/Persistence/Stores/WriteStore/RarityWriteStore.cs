using GameTools.Application.Abstractions.Stores.WriteStore;
using GameTools.Domain.Entities;

namespace GameTools.Infrastructure.Persistence.Stores.WriteStore
{
    public sealed class RarityWriteStore(AppDbContext db) : IRarityWriteStore
    {
        public async Task<Rarity?> GetByIdAsync(byte id, CancellationToken ct)
            => await db.Rarities.FindAsync([id], ct);

        public async Task AddAsync(Rarity entity, CancellationToken ct)
            => await db.Rarities.AddAsync(entity, ct);

        public void Remove(Rarity entity)
            => db.Rarities.Remove(entity);

        public void SetOriginalRowVersion(Rarity entity, string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            db.Entry(entity).Property(e => e.RowVersion).OriginalValue = bytes;
        }
    }
}
