using GameTools.Client.Application.Ports;
using GameTools.Client.Domain.Rarities;

namespace GameTools.Client.Application.UseCases.Rarities.UpdateRarity
{
    public sealed class UpdateRarityUseCase(IRarityGateway gateway)
    {
        public Task<Rarity> Handle(UpdateRarityInput input, CancellationToken ct)
            => gateway.UpdateAsync(input, ct);
    }
}
