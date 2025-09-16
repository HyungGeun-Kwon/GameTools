using GameTools.Client.Application.UseCases.Rarities.CreateRarity;
using GameTools.Client.Application.UseCases.Rarities.DeleteRarity;
using GameTools.Client.Application.UseCases.Rarities.UpdateRarity;
using GameTools.Client.Domain.Rarities;

namespace GameTools.Client.Application.Ports
{
    public interface IRarityGateway
    {
        Task<Rarity?> GetByIdAsync(byte id, CancellationToken ct);
        Task<IReadOnlyList<Rarity>> GetAllRaritiesAsync(CancellationToken ct);
        Task<Rarity> CreateAsync(CreateRarityInput input, CancellationToken ct);
        Task<Rarity> UpdateAsync(UpdateRarityInput input, CancellationToken ct);
        Task DeleteAsync(DeleteRarityInput input, CancellationToken ct);
    }
}
