using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Items.Models;
using GameTools.Server.Application.Features.Items.Queries.GetItemPage;
using GameTools.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence.Stores.ReadStore
{
    public sealed class ItemReadStore(AppDbContext db) : IItemReadStore
    {
        public Task<ItemReadModel?> GetByIdAsync(int id, CancellationToken ct)
            => db.Items.AsNoTracking()
                .Where(i => i.Id == id)
                .Select(i => new ItemReadModel(i.Id, i.Name, i.Price, i.Description, i.RarityId, i.Rarity.Grade, i.Rarity.ColorCode, i.RowVersion))
                .FirstOrDefaultAsync(ct);

        public async Task<IReadOnlyList<ItemReadModel>> GetByRarityIdAsync(byte rarityId, CancellationToken ct)
            => await db.Items.AsNoTracking()
                .Where(i => i.RarityId == rarityId)
                .Select(i => new ItemReadModel(i.Id, i.Name, i.Price, i.Description, i.RarityId, i.Rarity.Grade, i.Rarity.ColorCode, i.RowVersion))
                .ToListAsync(ct);

        public async Task<PagedResult<ItemReadModel>> GetPageAsync(ItemsPageSearchCriteria Criteria, CancellationToken ct)
        {
            IQueryable<Item> query = db.Items.AsNoTracking().AsQueryable();

            var search = Criteria.Filter?.Search;
            var rarityId = Criteria.Filter?.RarityId;
            var page = Criteria.Pagination.PageNumber;
            var size = Criteria.Pagination.PageSize;

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(i => i.Name.Contains(search.Trim()));

            if (rarityId.HasValue)
                query = query.Where(i => i.RarityId == rarityId.Value);

            query = query.OrderByDescending(i => i.Id);

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((page - 1) * size)
                .Take(size)
                .Select(i => new ItemReadModel(i.Id, i.Name, i.Price, i.Description, i.RarityId, i.Rarity.Grade, i.Rarity.ColorCode, i.RowVersion))
                .ToListAsync(ct);

            return new PagedResult<ItemReadModel>(items, total, page, size);
        }
    }
}
