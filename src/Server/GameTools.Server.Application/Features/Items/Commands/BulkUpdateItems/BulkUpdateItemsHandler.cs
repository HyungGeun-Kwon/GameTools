using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.BulkUpdateItems
{
    public sealed class BulkUpdateItemsHandler(IItemWriteStore writeStore)
        : IRequestHandler<BulkUpdateItemsCommand, BulkUpdateItemsResult>
    {
        public async Task<BulkUpdateItemsResult> Handle(BulkUpdateItemsCommand command, CancellationToken ct)
        {
            // BulkUpdateAsync handles its own transaction / SaveChanges.

            var updatedList = await writeStore.BulkUpdateAsync(command.Specs, ct);
            return new(updatedList);
        }
    }
}
