using GameTools.Client.Application.Ports;
using GameTools.Client.Domain.Items;

namespace GameTools.Client.Application.UseCases.Items.GetItemsByRarity
{
    public class GetItemByRarityUseCase(IItemsGateway gateway)
    {
        public Task<IReadOnlyList<Item>> Handle(byte rarityId, CancellationToken ct)
            => gateway.GetByRarityAsync(rarityId, ct);
    }
}
