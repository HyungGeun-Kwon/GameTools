using GameTools.Client.Application.Ports;
using GameTools.Client.Domain.Items;

namespace GameTools.Client.Application.UseCases.Items.GetItemById
{
    public class GetItemByIdUseCase(IItemsGateway gateway)
    {
        public Task<Item?> Handle(int id, CancellationToken ct)
            => gateway.GetByIdAsync(id, ct);
    }
}
