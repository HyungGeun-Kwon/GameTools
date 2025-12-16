using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.UnitOfWorks;
using MediatR;

namespace GameTools.Server.Application.Catalog.Items.Commands.BulkDeleteItems
{
    public sealed class BulkDeleteItemsHandler(IItemWriteStore writeStore, IUnitOfWork uow)
        : IRequestHandler<BulkDeleteItemsCommand, BulkDeleteItemsResult>
    {
        public async Task<BulkDeleteItemsResult> Handle(BulkDeleteItemsCommand command, CancellationToken ct)
        {
            await using var tx = await uow.BeginTransactionAsync(ct);

            var deletedList = await writeStore.BulkDeleteAsync(command.Specs, ct);

            await tx.CommitAsync(ct);
            return new(deletedList);
        }
    }
}
