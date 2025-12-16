using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.UnitOfWorks;
using MediatR;

namespace GameTools.Server.Application.Catalog.Items.Commands.BulkInsertItems
{
    public sealed class BulkInsertItemsHandler(IItemWriteStore writeStore, IUnitOfWork uow)
        : IRequestHandler<BulkInsertItemsCommand, BulkInsertItemsResult>
    {
        public async Task<BulkInsertItemsResult> Handle(BulkInsertItemsCommand command, CancellationToken ct)
        {
            await using var tx = await uow.BeginTransactionAsync(ct);

            var insertedList = await writeStore.BulkInsertAsync(command.Specs, ct);

            await tx.CommitAsync(ct);
            return new(insertedList);
        }
    }
}
