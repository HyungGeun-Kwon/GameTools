using GameTools.Application.Common.Paging;
using GameTools.Application.Features.Items.Dtos;

namespace GameTools.Application.Abstractions.ReadStore
{
    public interface IItemReadStore : IReadStore<ItemDto, int>
    {
        Task<PagedResult<ItemDto>> GetPageAsync(int page, int size, string? search, byte? rarityId, CancellationToken ct);
        Task<List<ItemDto>> GetByRarityIdAsync(byte rarityId, CancellationToken ct);
    }
}
