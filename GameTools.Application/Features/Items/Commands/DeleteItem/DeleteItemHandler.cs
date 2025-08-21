using GameTools.Application.Abstractions.Works;
using MediatR;
using GameTools.Application.Abstractions.Stores.WriteStore;

namespace GameTools.Application.Features.Items.Commands.DeleteItem
{
    public sealed class DeleteItemHandler(IItemWriteStore itemWriteStore, IUnitOfWork uow)
        : IRequestHandler<DeleteItemCommand>
    {
        public async Task Handle(DeleteItemCommand request, CancellationToken ct)
        {
            var item = await itemWriteStore.GetByIdAsync(request.ItemDeleteDto.Id, ct)
                       ?? throw new InvalidOperationException("Item not found.");

            itemWriteStore.SetOriginalRowVersion(item, request.ItemDeleteDto.RowVersionBase64);

            itemWriteStore.Remove(item);

            await uow.SaveChangesAsync(ct);
        }
    }
}
