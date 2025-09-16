using GameTools.Client.Application.Ports;
using GameTools.Client.Domain.Rarities;

namespace GameTools.Client.Application.UseCases.Rarities.GetAllRarities
{
    public sealed class GetAllRaritiesUseCase(IRarityGateway gateway)
    {
        public Task<IReadOnlyList<Rarity>> Handle(CancellationToken ct)
            => gateway.GetAllRaritiesAsync(ct);
    }
}
