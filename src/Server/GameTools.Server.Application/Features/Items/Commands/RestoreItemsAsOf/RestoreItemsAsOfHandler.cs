using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.RestoreItemsAsOf
{
    public sealed class RestoreItemsAsOfHandler(IItemWriteStore itemWriteStore)
        : IRequestHandler<RestoreItemsAsOfCommand, RestoreItemsAsOfResult>
    {
        public async Task<RestoreItemsAsOfResult> Handle(RestoreItemsAsOfCommand request, CancellationToken ct)
        {
            (Guid restoreId, int deleted, int inserted, int updated) = 
                await itemWriteStore.RestoreItemsAsOfAsync(request.Payload, ct);

            bool isChanged = (deleted + inserted + updated) > 0;
            return new RestoreItemsAsOfResult(restoreId, deleted, inserted, updated, isChanged);
        }
    }
}
