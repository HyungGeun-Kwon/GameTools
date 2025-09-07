using GameTools.Client.Application.Ports;

namespace GameTools.Client.Application.UseCases.Items.BulkUpdateItems
{
    public sealed class BulkUpdateItemsUseCase(IItemsGateway gateway)
    {
        public Task<BulkUpdateItemsOutput> Handle(BulkUpdateItemsInput input, CancellationToken ct)
            => gateway.BulkUpdateAsync(input, ct);
    }
}
