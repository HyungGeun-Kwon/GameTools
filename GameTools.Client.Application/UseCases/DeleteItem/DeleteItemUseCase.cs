using GameTools.Client.Application.Ports;
using GameTools.Client.Application.UseCases.DeleteItem;
using GameTools.Client.Domain.Items;

namespace GameTools.Client.Application.UseCases.CreateItem
{
    public sealed class DeleteItemUseCase(IItemsGateway gateway)
    {
        public Task Handle(DeleteItemInput input, CancellationToken ct)
            => gateway.DeleteAsync(input, ct);
    }
}
