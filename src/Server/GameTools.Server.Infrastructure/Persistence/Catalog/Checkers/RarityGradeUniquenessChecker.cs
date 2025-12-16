using GameTools.Server.Domain.Catalog.Rarities.Services;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence.Catalog.Checkers
{
    public sealed class RarityGradeUniquenessChecker(AppDbContext db)
        : IRarityGradeUniquenessChecker
    {
        public async Task<bool> ExistsAsync(RarityGrade value, CancellationToken ct)
        {
            return await db.Rarities
                .AsNoTracking()
                .AnyAsync(r => r.Grade == value, ct);
        }
    }
}
