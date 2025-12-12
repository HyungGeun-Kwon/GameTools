using System.Linq.Expressions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Features.Rarities.Models;
using GameTools.Server.Domain.Features.Rarities.Entities;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence.Catalog.Stores.ReadStore
{
    internal static class RarityQueryExtensions
    {
        private static readonly Expression<Func<Rarity, RarityReadModel>> RarityToReadModelExpr =
            rarity => new RarityReadModel(
                rarity.Id.Value,
                rarity.Grade.Value,
                rarity.ColorCode.Value,
                EF.Property<byte[]>(rarity, "RowVersion")
            );
        internal static IQueryable<RarityReadModel> SelectToReadModel(this IQueryable<Rarity> query)
            => query.Select(RarityToReadModelExpr);
    }
    public sealed class RarityReadStore(AppDbContext db) : IRarityReadStore
    {
        public Task<RarityReadModel?> GetByIdAsync(Guid id, CancellationToken ct)
            => db.Rarities.AsNoTracking()
                .Where(r => r.Id == RarityId.From(id))
                .SelectToReadModel()
                .FirstOrDefaultAsync(ct);

        public async Task<IReadOnlyList<RarityReadModel>> GetAllAsync(CancellationToken ct)
            => await db.Rarities.AsNoTracking()
                .OrderBy(r => r.Id)
                .SelectToReadModel()
                .ToListAsync(ct);
    }
}
