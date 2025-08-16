using GameTools.Application.Abstractions.ReadStore;
using GameTools.Application.Common.Paging;
using GameTools.Application.Features.Items.Dtos;
using GameTools.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Infrastructure.Persistence.ReadStore
{
    public sealed class ItemReadStore(AppDbContext db) : IItemReadStore
    {
        public async Task<ItemDto?> GetByIdAsync(int id, CancellationToken ct)
            => await db.Items.AsNoTracking()
                .Where(i => i.Id == id)
                .Select(i => new ItemDto(i.Id, i.Name, i.Price, i.Description, i.RarityId, i.Rarity.Grade, i.Rarity.ColorCode))
                .FirstOrDefaultAsync(ct);

        public async Task<List<ItemDto>> GetByRarityIdAsync(byte rarityId, CancellationToken ct)
            => await db.Items.AsNoTracking()
                .Where(i => i.RarityId == rarityId)
                .Select(i => new ItemDto(i.Id, i.Name, i.Price, i.Description, i.RarityId, i.Rarity.Grade, i.Rarity.ColorCode))
                .ToListAsync(ct);

        public async Task<PagedResult<ItemDto>> GetPageAsync(int page, int size, string? search, byte? rarityId, CancellationToken ct)
        {
            IQueryable<Item> query = db.Items.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(i => i.Name.Contains(search.Trim()));

            if (rarityId.HasValue)
                query = query.Where(i => i.RarityId == rarityId.Value);

            query = query.OrderByDescending(i => i.Id);

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((page - 1) * size).Take(size)
                .Select(i => new ItemDto(i.Id, i.Name, i.Price, i.Description, i.RarityId, i.Rarity.Grade, i.Rarity.ColorCode))
                .ToListAsync(ct);

            return new PagedResult<ItemDto>(items, total, page, size);
        }
    }
}
