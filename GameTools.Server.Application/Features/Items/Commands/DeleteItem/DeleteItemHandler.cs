using GameTools.Server.Application.Abstractions.Works;
using MediatR;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;

namespace GameTools.Server.Application.Features.Items.Commands.DeleteItem
{
    public sealed class DeleteItemHandler(IItemWriteStore itemWriteStore, IUnitOfWork uow)
        : IRequestHandler<DeleteItemCommand>
    {
        public async Task Handle(DeleteItemCommand request, CancellationToken ct)
        {
            var item = await itemWriteStore.GetByIdAsync(request.Payload.Id, ct)
                       ?? throw new InvalidOperationException("Item not found.");

            itemWriteStore.SetOriginalRowVersion(item, request.Payload.RowVersion);

            itemWriteStore.Remove(item);

            await uow.SaveChangesAsync(ct);
        }
    }
}
