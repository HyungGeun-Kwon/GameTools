using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Items.Models;
using GameTools.Server.Application.Features.Items.Queries.GetItemsPage;

namespace GameTools.Server.Application.Abstractions.Stores.ReadStore
{
    public interface IItemReadStore : IReadStore<ItemReadModel, Guid>
    {
        Task<PagedResult<ItemReadModel>> GetPageAsync(ItemsPageCriteria criteria, CancellationToken ct);
    }
}
