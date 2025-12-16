using GameTools.Server.Domain.Catalog.Rarities.Services;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence.Catalog.Checkers
{
    public sealed class RarityColorCodeUniquenessChecker(AppDbContext db)
        : IRarityColorCodeUniquenessChecker
    {
        public async Task<bool> ExistsAsync(RarityColorCode value, CancellationToken ct)
        {
            return await db.Rarities
                .AsNoTracking()
                .AnyAsync(r => r.ColorCode == value, ct);
        }
    }
}
