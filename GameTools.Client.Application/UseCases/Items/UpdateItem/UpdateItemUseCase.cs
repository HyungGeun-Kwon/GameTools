using GameTools.Client.Application.Ports;
using GameTools.Client.Domain.Items;

namespace GameTools.Client.Application.UseCases.Items.UpdateItem
{
    public sealed class UpdateItemUseCase(IItemsGateway gateway)
    {
        public Task<Item> Handle(UpdateItemInput input, CancellationToken ct)
            => gateway.UpdateAsync(input, ct);
    }
}
