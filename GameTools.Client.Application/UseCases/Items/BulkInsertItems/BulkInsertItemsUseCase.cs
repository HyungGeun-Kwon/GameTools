using GameTools.Client.Application.Ports;

namespace GameTools.Client.Application.UseCases.Items.BulkInsertItems
{
    public sealed class BulkInsertItemsUseCase(IItemsGateway gateway)
    {
        public Task<BulkInsertItemsOutput> Handle(BulkInsertItemsInput input, CancellationToken ct)
            => gateway.BulkInsertAsync(input, ct);
    }
}
