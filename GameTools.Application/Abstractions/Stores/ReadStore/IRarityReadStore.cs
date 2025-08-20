using GameTools.Application.Features.Rarities.Dtos;

namespace GameTools.Application.Abstractions.Stores.ReadStore
{
    public interface IRarityReadStore : IReadStore<RarityDto, byte>
    {
        Task<List<RarityDto>> GetAllAsync(CancellationToken ct);
    }
}
