using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.UnitOfWorks;
using MediatR;

namespace GameTools.Server.Application.Catalog.Items.Commands.BulkUpdateItems
{
    public sealed class BulkUpdateItemsHandler(IItemWriteStore writeStore, IUnitOfWork uow)
        : IRequestHandler<BulkUpdateItemsCommand, BulkUpdateItemsResult>
    {
        public async Task<BulkUpdateItemsResult> Handle(BulkUpdateItemsCommand command, CancellationToken ct)
        {
            await using var tx = await uow.BeginTransactionAsync(ct);

            var updatedList = await writeStore.BulkUpdateAsync(command.Specs, ct);

            await tx.CommitAsync(ct);
            return new(updatedList);
        }
    }
}
