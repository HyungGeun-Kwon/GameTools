using GameTools.Client.Application.Ports;
using GameTools.Client.Domain.Rarities;

namespace GameTools.Client.Application.UseCases.Rarities.GetRarityById
{
    public sealed class GetRarityByIdUseCase(IRarityGateway gateway)
    {
        public async Task<Rarity?> Handle(byte id, CancellationToken ct)
            => await gateway.GetByIdAsync(id, ct);
    }
}
