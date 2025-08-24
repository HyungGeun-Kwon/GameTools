using GameTools.Client.Application.Ports;
using GameTools.Client.Application.UseCases.DeleteItem;
using GameTools.Client.Domain.Items;

namespace GameTools.Client.Application.UseCases.CreateItem
{
    public sealed class UpdateItemUseCase(IItemsGateway gateway)
    {
        public Task<Item> Handle(UpdateItemInput input, CancellationToken ct)
            => gateway.UpdateAsync(input, ct);
    }
}
