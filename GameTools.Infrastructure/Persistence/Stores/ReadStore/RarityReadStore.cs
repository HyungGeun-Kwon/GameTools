using GameTools.Application.Abstractions.Stores.ReadStore;
using GameTools.Application.Features.Rarities.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Infrastructure.Persistence.Stores.ReadStore
{
    public sealed class RarityReadStore(AppDbContext db) : IRarityReadStore
    {
        public async Task<IReadOnlyList<RarityDto>> GetAllAsync(CancellationToken ct)
            => await db.Rarities.AsNoTracking()
                .OrderBy(r => r.Id)
                .Select(r => new RarityDto(r.Id, r.Grade, r.ColorCode, Convert.ToBase64String(r.RowVersion)))
                .ToListAsync(ct);

        public async Task<RarityDto?> GetByIdAsync(byte id, CancellationToken ct)
            => await db.Rarities.AsNoTracking()
                .Where(r => r.Id == id)
                .Select(r => new RarityDto(r.Id, r.Grade, r.ColorCode, Convert.ToBase64String(r.RowVersion)))
                .FirstOrDefaultAsync(ct);
    }
}
