using GameTools.Client.Application.Ports;
using GameTools.Client.Domain.Rarities;

namespace GameTools.Client.Application.UseCases.Rarities.GetRarityById
{
    public sealed class GetRarityByIdUseCase(IRarityGateway gateway)
    {
        public Task<Rarity?> Handle(byte id, CancellationToken ct)
            => gateway.GetByIdAsync(id, ct);
    }
}
