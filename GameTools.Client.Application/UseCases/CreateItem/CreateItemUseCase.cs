using GameTools.Client.Application.Ports;
using GameTools.Client.Domain.Items;

namespace GameTools.Client.Application.UseCases.CreateItem
{
    public sealed class CreateItemUseCase(IItemsGateway gateway)
    {
        public Task<Item> Handle(CreateItemInput input, CancellationToken ct)
            => gateway.CreateAsync(input, ct);
    }
}
