using GameTools.Client.Application.Ports;

namespace GameTools.Client.Application.UseCases.Items.BulkDeleteItems
{
    public sealed class BulkDeleteItemsUseCase(IItemsGateway gateway)
    {
        public Task<BulkDeleteItemsOutput> Handle(BulkDeleteItemsInput input, CancellationToken ct)
            => gateway.BulkDeleteAsync(input, ct);
    }
}
