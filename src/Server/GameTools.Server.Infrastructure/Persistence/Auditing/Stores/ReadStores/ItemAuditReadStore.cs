using System.Data;
using System.Linq.Expressions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage;
using GameTools.Server.Infrastructure.Persistence.Auditing.Models;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence.Auditing.Stores.ReadStores
{
    internal static class ItemAuditQueryExtensions
    {
        private static readonly Expression<Func<ItemAudit, ItemAuditReadModel>> ItemAuditToReadModelExpr =
            itemAudit => new ItemAuditReadModel(
                itemAudit.AuditId,
                itemAudit.ItemId,
                itemAudit.Action.ToString(),
                itemAudit.BeforeJson,
                itemAudit.AfterJson,
                itemAudit.ChangedAtUtc,
                itemAudit.ChangedBy ?? "");

        internal static IQueryable<ItemAuditReadModel> SelectToReadModel(this IQueryable<ItemAudit> query)
            => query.Select(ItemAuditToReadModelExpr);
    }
    public sealed class ItemAuditReadStore(AppDbContext db) : IItemAuditReadStore
    {
        public Task<ItemAuditReadModel?> GetByIdAsync(Guid id, CancellationToken ct)
            => db.ItemAudits
                .AsNoTracking()
                .Where(i => i.AuditId == id)
                .SelectToReadModel()
                .SingleOrDefaultAsync(ct);

        public async Task<PagedResult<ItemAuditReadModel>> GetItemAuditPageAsync(ItemAuditPageCriteria criteria, CancellationToken ct)
        {
            IQueryable<ItemAudit> query = db.ItemAudits.AsNoTracking();

            if (criteria.Filter is { } filter)
            {
                var itemId = criteria.Filter.ItemId;
                var fromUtc = criteria.Filter.FromUtc;
                var toUtc = criteria.Filter.ToUtc;
                var actions = criteria.Filter.Actions;

                if (itemId is not null) query = query.Where(i => i.ItemId == itemId);

                if (fromUtc is not null) query = query.Where(i => i.ChangedAtUtc >= fromUtc);

                if (toUtc is not null) query = query.Where(i => i.ChangedAtUtc < toUtc);
                
                if (actions is { Count: > 0 })
                    query = query.Where(i => actions.Contains(i.Action.ToString()));
            }

            var skip = (criteria.Pagination.PageNumber - 1) * criteria.Pagination.PageSize;
            var take = criteria.Pagination.PageSize;

            var total = await query.CountAsync(ct);

            query = ApplySorting(query, criteria.SortBy, criteria.Desc);

            var itemAudits = await query
                .Skip(skip)
                .Take(take)
                .SelectToReadModel()
                .ToListAsync(ct);

            return new PagedResult<ItemAuditReadModel>(
                itemAudits,
                total,
                criteria.Pagination.PageNumber,
                criteria.Pagination.PageSize);
        }

        private static IQueryable<ItemAudit> ApplySorting(
            IQueryable<ItemAudit> query,
            string? sortBy,
            bool desc)
        {
            sortBy = string.IsNullOrWhiteSpace(sortBy) ? "id" : sortBy.Trim().ToLowerInvariant();

            return sortBy switch
            {
                "itemid" => desc ? query.OrderByDescending(i => i.ItemId) : query.OrderBy(i => i.ItemId),
                "changedatutc" => desc ? query.OrderByDescending(i => i.ChangedAtUtc) : query.OrderBy(i => i.ChangedAtUtc),
                _ => desc ? query.OrderByDescending(i => i.AuditId) : query.OrderBy(i => i.AuditId),
            };
        }
    }
}
