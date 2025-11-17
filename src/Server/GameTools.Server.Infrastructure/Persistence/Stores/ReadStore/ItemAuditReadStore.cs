using System.Data;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage;
using GameTools.Server.Domain.Auditing;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence.Stores.ReadStore
{
    public sealed class ItemAuditReadStore(AppDbContext db) : IItemAuditReadStore
    {
        public async Task<PagedResult<ItemAuditReadModel>> GetItemAuditPageAsync(ItemAuditPageSearchCriteria criteria, CancellationToken ct)
        {
            IQueryable<ItemAudit> query = db.ItemAudits.AsNoTracking().AsQueryable();

            var pagination = criteria.Pagination;

            var itemId = criteria.Filter?.ItemId;
            var fromUtc = criteria.Filter?.FromUtc;
            var toUtc = criteria.Filter?.ToUtc;
            var action = criteria.Filter?.Action;

            if (itemId.HasValue) query = query.Where(x => x.ItemId == itemId);
            if (fromUtc.HasValue) query = query.Where(x => x.ChangedAtUtc >= fromUtc);
            if (toUtc.HasValue) query = query.Where(x => x.ChangedAtUtc < toUtc);
            if (!string.IsNullOrWhiteSpace(action)) query = query.Where(x => x.Action.ToString() == action);

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(x => x.ChangedAtUtc)
                .ThenByDescending(x => x.AuditId)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(x => new ItemAuditReadModel(
                    x.AuditId,
                    x.ItemId,
                    x.Action.ToString(),
                    x.BeforeJson,
                    x.AfterJson,
                    x.ChangedAtUtc,
                    x.ChangedBy ?? ""
                ))
                .ToListAsync(ct);

            return new PagedResult<ItemAuditReadModel>(items, total, pagination.PageNumber, pagination.PageSize);
        }
    }
}
