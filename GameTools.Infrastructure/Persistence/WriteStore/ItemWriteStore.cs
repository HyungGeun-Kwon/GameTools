using GameTools.Application.Abstractions.WriteStore;
using GameTools.Domain.Entities;

namespace GameTools.Infrastructure.Persistence.WriteStore
{
    public sealed class ItemWriteStore(AppDbContext db) : IItemWriteStore
    {
        public async Task<Item?> GetByIdAsync(int id, CancellationToken ct)
            => await db.Items.FindAsync([id], ct);

        public async Task AddAsync(Item entity, CancellationToken ct)
            => await db.Items.AddAsync(entity, ct);

        public void Remove(Item entity)
            => db.Items.Remove(entity);
    }
}
