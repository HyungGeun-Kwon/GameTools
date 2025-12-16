using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Operations.Restores.Queries.GetItemRestoreHistoriesPage;

namespace GameTools.Server.Application.Abstractions.Stores.ReadStore
{
    public interface IItemRestoreHistoryReadStore : IReadStore<ItemRestoreHistoriesReadModel, Guid>
    {
        Task<PagedResult<ItemRestoreHistoriesReadModel>> GetItemRestoreHistoriesPageAsync(ItemRestoreHistoriesPageCriteria criteria, CancellationToken ct);
    }
}
