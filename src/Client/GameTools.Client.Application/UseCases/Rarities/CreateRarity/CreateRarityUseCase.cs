using GameTools.Client.Application.Ports;
using GameTools.Client.Domain.Rarities;

namespace GameTools.Client.Application.UseCases.Rarities.CreateRarity
{
    public sealed class CreateRarityUseCase(IRarityGateway gateway)
    {
        public Task<Rarity> Handle(CreateRarityInput input, CancellationToken ct)
            => gateway.CreateAsync(input, ct);
    }
}