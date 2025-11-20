using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using MediatR;

namespace GameTools.Server.Application.Features.Restores.Commands.RestoreItems
{
    public sealed class RestoreItemsHandler(IRestoreItemWriteStore restoreItemWriteStore)
        : IRequestHandler<RestoreItemsCommand, RestoreItemsResult>
    {
        public async Task<RestoreItemsResult> Handle(RestoreItemsCommand request, CancellationToken ct)
        {
            var storeResult = await restoreItemWriteStore.RestoreItemsAsOfAsync(request.Spec, ct);

            bool isChanged = storeResult.Deleted + storeResult.Inserted + storeResult.Updated > 0;

            return new RestoreItemsResult(
                storeResult.RestoreId,
                storeResult.Deleted,
                storeResult.Inserted,
                storeResult.Updated,
                isChanged);
        }
    }
}
