using GameTools.Server.Application.Operations.Restores.Commands.RestoreItems;

namespace GameTools.Server.Application.Abstractions.Stores.WriteStore
{
    public interface IRestoreItemWriteStore
    {
        Task<RestoreItemsStoreResult> RestoreItemsAsOfAsync(
            RestoreItemsSpec spec, CancellationToken ct);
    }
}
