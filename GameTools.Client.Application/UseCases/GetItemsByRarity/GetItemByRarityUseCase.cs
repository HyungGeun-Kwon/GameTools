using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTools.Client.Application.Ports;
using GameTools.Client.Domain.Items;

namespace GameTools.Client.Application.UseCases.GetByRarity
{
    public class GetItemByRarityUseCase(IItemsGateway gateway)
    {
        public Task<IReadOnlyList<Item>> Handle(byte rarityId, CancellationToken ct)
            => gateway.GetByRarityAsync(rarityId, ct);
    }
}
