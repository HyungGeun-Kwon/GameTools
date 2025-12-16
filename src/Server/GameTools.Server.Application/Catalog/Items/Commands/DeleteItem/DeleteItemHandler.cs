using GameTools.Server.Application.Abstractions.Exceptions;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.UnitOfWorks;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;
using MediatR;

namespace GameTools.Server.Application.Catalog.Items.Commands.DeleteItem
{
    public sealed class DeleteItemHandler(
        IItemWriteStore itemWriteStore,
        IUnitOfWork uow) 
        : IRequestHandler<DeleteItemCommand, DeleteItemResult>
    {
        public async Task<DeleteItemResult> Handle(DeleteItemCommand command, CancellationToken ct)
        {
            var item = await itemWriteStore.LoadForUpdateAsync(ItemId.From(command.Spec.Id), ct)
                ?? throw new NotFoundException($"Item '{command.Spec.Id}' not found.");

            itemWriteStore.SetOriginalRowVersion(item, command.Spec.RowVersion);
            itemWriteStore.Remove(item);

            await uow.SaveChangesAsync(ct);

            return new DeleteItemResult();
        }
    }
}
