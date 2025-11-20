using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.BulkDeleteItems
{
    public sealed class BulkDeleteItemsHandler(IItemWriteStore writeStore)
        : IRequestHandler<BulkDeleteItemsCommand, BulkDeleteItemsResult>
    {
        public async Task<BulkDeleteItemsResult> Handle(BulkDeleteItemsCommand command, CancellationToken ct)
        {
            // BulkDeleteAsync handles its own transaction / SaveChanges.

            var deletedList = await writeStore.BulkDeleteAsync(command.Specs, ct);
            return new(deletedList);
        }
    }
}
