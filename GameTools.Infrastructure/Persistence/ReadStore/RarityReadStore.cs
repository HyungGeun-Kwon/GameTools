using GameTools.Application.Abstractions.ReadStore;
using GameTools.Application.Features.Rarities.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Infrastructure.Persistence.ReadStore
{
    public sealed class RarityReadStore(AppDbContext db) : IRarityReadStore
    {
        public async Task<List<RarityDto>> GetAllAsync(CancellationToken ct)
            => await db.Rarities.AsNoTracking()
                .OrderBy(r => r.Id)
                .Select(r => new RarityDto(r.Id, r.Grade, r.ColorCode))
                .ToListAsync(ct);

        public async Task<RarityDto?> GetByIdAsync(byte id, CancellationToken ct)
            => await db.Rarities.AsNoTracking()
                .Where(r => r.Id == id)
                .Select(r => new RarityDto(r.Id, r.Grade, r.ColorCode))
                .FirstOrDefaultAsync(ct);
    }
}
