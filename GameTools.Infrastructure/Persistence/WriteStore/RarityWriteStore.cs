using GameTools.Application.Abstractions.WriteStore;
using GameTools.Domain.Entities;

namespace GameTools.Infrastructure.Persistence.WriteStore
{
    public sealed class RarityWriteStore(AppDbContext db) : IRarityWriteStore
    {
        public async Task<Rarity?> GetByIdAsync(byte id, CancellationToken ct)
            => await db.Rarities.FindAsync([id], ct);

        public async Task AddAsync(Rarity entity, CancellationToken ct)
            => await db.Rarities.AddAsync(entity, ct);

        public void Remove(Rarity entity)
            => db.Rarities.Remove(entity);
    }
}
