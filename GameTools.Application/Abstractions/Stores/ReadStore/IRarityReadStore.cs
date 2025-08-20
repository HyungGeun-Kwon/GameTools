using GameTools.Application.Features.Rarities.Dtos;

namespace GameTools.Application.Abstractions.Stores.ReadStore
{
    public interface IRarityReadStore : IReadStore<RarityDto, byte>
    {
        Task<IReadOnlyList<RarityDto>> GetAllAsync(CancellationToken ct);
    }
}
