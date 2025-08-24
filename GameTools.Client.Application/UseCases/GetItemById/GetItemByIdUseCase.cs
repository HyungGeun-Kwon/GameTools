using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTools.Client.Application.Ports;
using GameTools.Client.Domain.Items;

namespace GameTools.Client.Application.UseCases.GetItemById
{
    public class GetItemByIdUseCase(IItemsGateway gateway)
    {
        public Task<Item?> Handle(int id, CancellationToken ct)
            => gateway.GetByIdAsync(id, ct);
    }
}
