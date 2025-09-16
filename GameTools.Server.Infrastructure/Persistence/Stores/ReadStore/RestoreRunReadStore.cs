using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Restores.Queries.GetRestoreRunsPage;
using GameTools.Server.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence.Stores.ReadStore
{
    public sealed class RestoreRunReadStore(AppDbContext db) : IRestoreRunReadStore
    {
        public async Task<PagedResult<RestoreRunReadModel>> GetRestoreRunsPageAsync(
            RestoreRunPageSearchCriteria criteria, CancellationToken ct)
        {
            IQueryable<RestoreRunRow> query = db.RestoreRunRows.AsNoTracking().AsQueryable();

            var pagination = criteria.Pagination;
            var actor = criteria.Filter?.Actor;
            var toUtc = criteria.Filter?.ToUtc;
            var fromUtc = criteria.Filter?.FromUtc;
            var dryOnly = criteria.Filter?.DryOnly;

            if (fromUtc is not null) query = query.Where(x => x.StartedAtUtc >= fromUtc);
            if (toUtc is not null) query = query.Where(x => x.StartedAtUtc < toUtc);
            if (!string.IsNullOrWhiteSpace(actor))
                query = query.Where(x => x.Actor == actor);
            if (dryOnly is not null)
                query = query.Where(x => x.DryRun == dryOnly.Value);

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(x => x.StartedAtUtc)
                .ThenByDescending(x => x.RestoreId)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(x => new RestoreRunReadModel(
                    x.RestoreId,
                    x.AsOfUtc,
                    x.Actor,
                    x.DryRun,
                    x.StartedAtUtc,
                    x.EndedAtUtc,
                    x.AffectedCounts,
                    x.Notes,
                    x.FiltersJson
                ))
                .ToListAsync(ct);

            return new PagedResult<RestoreRunReadModel>(items, total, pagination.PageNumber, pagination.PageSize);
        }
    }
}
