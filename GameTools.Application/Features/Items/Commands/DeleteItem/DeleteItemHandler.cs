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
            var item = await itemWriteStore.GetByIdAsync(request.Id, ct)
                       ?? throw new InvalidOperationException("Item not found.");

            itemWriteStore.Remove(item);
            await uow.SaveChangesAsync(ct);
        }
    }
}
