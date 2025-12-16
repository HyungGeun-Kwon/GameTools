using GameTools.Server.Application.Catalog.Items.Models;
using GameTools.Server.Application.Catalog.Items.Queries.GetItemsPage;
using GameTools.Server.Application.Common.Paging;

namespace GameTools.Server.Application.Abstractions.Stores.ReadStore
{
    public interface IItemReadStore : IReadStore<ItemReadModel, Guid>
    {
        Task<PagedResult<ItemReadModel>> GetPageAsync(ItemsPageCriteria criteria, CancellationToken ct);
    }
}
