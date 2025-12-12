using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Domain.Features.Rarities.Entities;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence.Catalog.Stores.WriteStore
{
    public sealed class RarityWriteStore(AppDbContext db) : IRarityWriteStore
    {
        public async Task AddAsync(Rarity aggregate, CancellationToken ct)
            => await db.Rarities.AddAsync(aggregate, ct);

        public void Remove(Rarity aggregate)
            => db.Rarities.Remove(aggregate);

        public byte[] GetRowVersion(Rarity aggregate)
        {
            var property = db.Entry(aggregate).Property<byte[]>("RowVersion");

            return property.CurrentValue ?? property.OriginalValue;
        }

        public Task<Rarity?> LoadForUpdateAsync(RarityId id, CancellationToken ct)
            => db.Rarities.SingleOrDefaultAsync(i => i.Id == id, ct);

        public void SetOriginalRowVersion(Rarity aggregate, byte[] rowVersion)
            => db.Entry(aggregate).Property<byte[]>("RowVersion").OriginalValue = rowVersion;
    }
}
