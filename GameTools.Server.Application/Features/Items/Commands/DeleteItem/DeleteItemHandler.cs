using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.Works;
using GameTools.Server.Application.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Application.Features.Items.Commands.DeleteItem
{
    public sealed class DeleteItemHandler(IItemWriteStore itemWriteStore, IUnitOfWork uow)
        : IRequestHandler<DeleteItemCommand, WriteStatusCode>
    {
        public async Task<WriteStatusCode> Handle(DeleteItemCommand request, CancellationToken ct)
        {
            var item = await itemWriteStore.GetByIdAsync(request.Payload.Id, ct);

            if (item == null)
                return WriteStatusCode.NotFound;

            itemWriteStore.SetOriginalRowVersion(item, request.Payload.RowVersion);

            itemWriteStore.Remove(item);

            try
            {
                await uow.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await itemWriteStore.GetByIdAsync(request.Payload.Id, ct);
                return exists == null ? WriteStatusCode.NotFound : WriteStatusCode.VersionMismatch;
            }

            return WriteStatusCode.Success;
        }
    }
}
