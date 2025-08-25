using GameTools.Client.Application.Ports;
using GameTools.Client.Domain.Rarities;

namespace GameTools.Client.Application.UseCases.Rarities.UpdateRarity
{
    public sealed class UpdateRarityUseCase(IRarityGateway gateway)
    {
        public async Task<Rarity> Handle(UpdateRarityInput input, CancellationToken ct)
            => await gateway.UpdateAsync(input, ct);
    }
}
