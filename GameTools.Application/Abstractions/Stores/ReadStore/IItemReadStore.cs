using GameTools.Application.Common.Paging;
using GameTools.Application.Features.Items.Dtos;
using GameTools.Application.Features.Items.Queries.GetItemPage;

namespace GameTools.Application.Abstractions.Stores.ReadStore
{
    public interface IItemReadStore : IReadStore<ItemDto, int>
    {
        Task<PagedResult<ItemDto>> GetPageAsync(GetItemsPageQueryParams getItemsPageQueryParams, CancellationToken ct);
        Task<List<ItemDto>> GetByRarityIdAsync(byte rarityId, CancellationToken ct);
    }
}
