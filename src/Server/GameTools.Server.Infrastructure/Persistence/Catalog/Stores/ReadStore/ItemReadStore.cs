using System.Linq.Expressions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Catalog.Items.Models;
using GameTools.Server.Application.Catalog.Items.Queries.GetItemsPage;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Domain.Catalog.Items.Entities;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence.Catalog.Stores.ReadStore
{
    internal static class ItemQueryExtensions
    {
        private static readonly Expression<Func<Item, ItemReadModel>> ItemToReadModelExpr =
            item => new ItemReadModel(
                item.Id.Value,
                item.Name.Value,
                item.Price.Value,
                item.Description.Value,
                item.RarityId.Value,
                EF.Property<byte[]>(item, "RowVersion")
            );
        internal static IQueryable<ItemReadModel> SelectToReadModel(this IQueryable<Item> query)
            => query.Select(ItemToReadModelExpr);
    }
    public sealed class ItemReadStore(AppDbContext db) : IItemReadStore
    {
        public Task<ItemReadModel?> GetByIdAsync(Guid id, CancellationToken ct)
            => db.Items.AsNoTracking()
                .Where(i => i.Id == ItemId.From(id))
                .SelectToReadModel()
                .FirstOrDefaultAsync(ct);

        public async Task<IReadOnlyList<ItemReadModel>> GetByRarityIdAsync(Guid rarityId, CancellationToken ct)
            => await db.Items.AsNoTracking()
                .Where(i => i.RarityId == RarityId.From(rarityId))
                .SelectToReadModel()
                .ToListAsync(ct);

        public async Task<PagedResult<ItemReadModel>> GetPageAsync(ItemsPageCriteria criteria, CancellationToken ct)
        {
            IQueryable<Item> query = db.Items.AsNoTracking();

            if (criteria.Filter is { } filter)
            {
                if (!string.IsNullOrWhiteSpace(filter.Search))
                {
                    var trimedSearch = filter.Search.Trim();

                    query = query.Where(i =>
                        EF.Property<string>(i, nameof(Item.Name)).Contains(trimedSearch) ||
                        (EF.Property<string>(i, nameof(Item.Description)) ?? string.Empty).Contains(trimedSearch));
                }

                if (filter.RarityIds is { Count: > 0 })
                {
                    var rarityIds = filter.RarityIds
                        .Select(RarityId.From)
                        .ToList();
                    query = query.Where(i => rarityIds.Contains(i.RarityId));
                }
            }

            var skip = (criteria.Pagination.PageNumber - 1) * criteria.Pagination.PageSize;
            var take = criteria.Pagination.PageSize;

            var total = await query.CountAsync(ct);
            
            query = ApplySorting(query, criteria.SortBy, criteria.Desc);

            var items = await query
                .Skip(skip)
                .Take(take)
                .SelectToReadModel()
                .ToListAsync(ct);


            return new PagedResult<ItemReadModel>(
                items, 
                total, 
                criteria.Pagination.PageNumber, 
                criteria.Pagination.PageSize);
        }

        private static IQueryable<Item> ApplySorting(
            IQueryable<Item> query,
            string? sortBy,
            bool desc)
        {
            sortBy = string.IsNullOrWhiteSpace(sortBy) ? "id" : sortBy.Trim().ToLowerInvariant();

            return sortBy switch
            {
                "name" => desc ? query.OrderByDescending(i => EF.Property<string>(i, "Name"))
                                : query.OrderBy(i => EF.Property<string>(i, "Name")),
                "price" => desc ? query.OrderByDescending(i => EF.Property<int>(i, "Price"))
                                : query.OrderBy(i => EF.Property<int>(i, "Price")),
                _ => desc ? query.OrderByDescending(i => i.Id) : query.OrderBy(i => i.Id),
            };
        }
    }
}
