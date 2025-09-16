using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Items.Models;
using GameTools.Server.Application.Features.Items.Queries.GetItemPage;

namespace GameTools.Server.Application.Abstractions.Stores.ReadStore
{
    public interface IItemReadStore : IReadStore<ItemReadModel, int>
    {
        Task<PagedResult<ItemReadModel>> GetPageAsync(ItemsPageSearchCriteria Criteria, CancellationToken ct);
        Task<IReadOnlyList<ItemReadModel>> GetByRarityIdAsync(byte rarityId, CancellationToken ct);
    }
}
