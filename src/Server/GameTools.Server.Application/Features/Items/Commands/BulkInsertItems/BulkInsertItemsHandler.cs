using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.BulkInsertItems
{
    public sealed class BulkInsertItemsHandler(IItemWriteStore writeStore)
        : IRequestHandler<BulkInsertItemsCommand, BulkInsertItemsResult>
    {
        public async Task<BulkInsertItemsResult> Handle(BulkInsertItemsCommand command, CancellationToken ct)
        {
            // BulkInsertAsync handles its own transaction / SaveChanges.

            var insertedList = await writeStore.BulkInsertAsync(command.Specs, ct);
            return new(insertedList);
        }
    }
}
