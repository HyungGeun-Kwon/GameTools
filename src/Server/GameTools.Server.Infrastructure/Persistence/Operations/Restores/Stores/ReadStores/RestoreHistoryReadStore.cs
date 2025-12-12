using System.Linq.Expressions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Restores.Queries.GetItemRestoreHistoriesPage;
using GameTools.Server.Infrastructure.Persistence.Operations.Restores.Models;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence.Operations.Restores.Stores.ReadStores
{
    internal static class ItemRestoreHistoryQueryExtensions
    {
        private static readonly Expression<Func<RestoreHistory, ItemRestoreHistoriesReadModel>> ItemRestoreHistoryToReadModelExpr =
            history => new ItemRestoreHistoriesReadModel(
                history.RestoreId,
                    history.AsOfUtc,
                    history.Actor,
                    history.DryRun,
                    history.StartedAtUtc,
                    history.EndedAtUtc,
                    history.AffectedCounts,
                    history.Notes,
                    history.FiltersJson);

        internal static IQueryable<ItemRestoreHistoriesReadModel> SelectToReadModel(this IQueryable<RestoreHistory> query)
            => query.Select(ItemRestoreHistoryToReadModelExpr);
    }
    public sealed class ItemRestoreHistoryReadStore(AppDbContext db) : IItemRestoreHistoryReadStore
    {
        public Task<ItemRestoreHistoriesReadModel?> GetByIdAsync(Guid id, CancellationToken ct)
            => db.ItemRestoreHistories
                .AsNoTracking()
                .Where(h => h.RestoreId == id)
                .SelectToReadModel()
                .SingleOrDefaultAsync(ct);

        public async Task<PagedResult<ItemRestoreHistoriesReadModel>> GetItemRestoreHistoriesPageAsync(
            ItemRestoreHistoriesPageCriteria criteria, CancellationToken ct)
        {
            var query = db.ItemRestoreHistories.AsNoTracking();

            if (criteria.Filter is { } filter)
            {
                var fromUtc = criteria.Filter.FromUtc;
                var toUtc = criteria.Filter.ToUtc;
                var actors = criteria.Filter.Actors;
                var dryOnly = criteria.Filter.DryOnly;

                if (fromUtc is not null) query = query.Where(h => h.StartedAtUtc >= fromUtc);
                
                if (toUtc is not null) query = query.Where(h => h.StartedAtUtc < toUtc);
                
                if (actors is { Count: > 0 })
                    query = query.Where(h => actors.Contains(h.Actor));

                if (dryOnly is not null)
                    query = query.Where(h => h.DryRun == dryOnly.Value);
            }

            var skip = (criteria.Pagination.PageNumber - 1) * criteria.Pagination.PageSize;
            var take = criteria.Pagination.PageSize;

            var total = await query.CountAsync(ct);

            query = ApplySorting(query, criteria.SortBy, criteria.Desc);

            var histories = await query
                .Skip(skip)
                .Take(take)
                .SelectToReadModel()
                .ToListAsync(ct);

            return new PagedResult<ItemRestoreHistoriesReadModel>(
                histories,
                total,
                criteria.Pagination.PageNumber,
                criteria.Pagination.PageSize);
        }

        private static IQueryable<RestoreHistory> ApplySorting(
            IQueryable<RestoreHistory> query,
            string? sortBy,
            bool desc)
        {
            sortBy = string.IsNullOrWhiteSpace(sortBy) ? "id" : sortBy.Trim().ToLowerInvariant();

            return sortBy switch
            {
                "id" => desc ? query.OrderByDescending(h => h.RestoreId) : query.OrderBy(i => i.RestoreId),
                "asofutc" => desc ? query.OrderByDescending(h => h.AsOfUtc) : query.OrderBy(h => h.AsOfUtc),
                "Actor" => desc ? query.OrderByDescending(h => h.Actor) : query.OrderBy(h => h.Actor),
                "StartedAtUtc" => desc ? query.OrderByDescending(h => h.StartedAtUtc) : query.OrderBy(h => h.StartedAtUtc),
                "EndedAtUtc" => desc ? query.OrderByDescending(h => h.EndedAtUtc) : query.OrderBy(h => h.EndedAtUtc),
                "AffectedCounts" => desc ? query.OrderByDescending(h => h.AffectedCounts) : query.OrderBy(h => h.AffectedCounts),
                _ => desc ? query.OrderByDescending(h => h.RestoreId) : query.OrderBy(h => h.RestoreId),
            };
        }
    }
}
