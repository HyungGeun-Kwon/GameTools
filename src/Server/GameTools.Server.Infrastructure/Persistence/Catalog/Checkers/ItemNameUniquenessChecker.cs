using GameTools.Server.Domain.Catalog.Items.Services;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence.Catalog.Checkers
{
    public sealed class ItemNameUniquenessChecker(AppDbContext db)
        : IItemNameUniquenessChecker
    {
        public async Task<bool> ExistsAsync(ItemName value, CancellationToken ct)
        {
            return await db.Items
                .AsNoTracking()
                .AnyAsync(i => i.Name == value, ct);
        }
    }
}
