using GameTools.Client.Application.Ports;

namespace GameTools.Client.Application.UseCases.Items.BulkUpdateItems
{
    public sealed class BulkUpdateItemsUseCase(IItemsGateway gateway)
    {
        public async Task<BulkUpdateItemsOutput> Handle(BulkUpdateItemsInput input, CancellationToken ct)
            => await gateway.BulkUpdateAsync(input, ct);
    }
}
