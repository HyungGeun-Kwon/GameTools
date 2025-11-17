using GameTools.Client.Application.Ports;

namespace GameTools.Client.Application.UseCases.Items.DeleteItem
{
    public sealed class DeleteItemUseCase(IItemsGateway gateway)
    {
        public Task Handle(DeleteItemInput input, CancellationToken ct)
            => gateway.DeleteAsync(input, ct);
    }
}
