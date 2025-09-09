using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Features.Rarities.Models;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence.Stores.ReadStore
{
    public sealed class RarityReadStore(AppDbContext db) : IRarityReadStore
    {
        public async Task<IReadOnlyList<RarityReadModel>> GetAllAsync(CancellationToken ct)
            => await db.Rarities.AsNoTracking()
                .OrderBy(r => r.Id)
                .Select(r => new RarityReadModel(r.Id, r.Grade, r.ColorCode, r.RowVersion))
                .ToListAsync(ct);

        public Task<RarityReadModel?> GetByIdAsync(byte id, CancellationToken ct)
            => db.Rarities.AsNoTracking()
                .Where(r => r.Id == id)
                .Select(r => new RarityReadModel(r.Id, r.Grade, r.ColorCode, r.RowVersion))
                .FirstOrDefaultAsync(ct);
    }
}
