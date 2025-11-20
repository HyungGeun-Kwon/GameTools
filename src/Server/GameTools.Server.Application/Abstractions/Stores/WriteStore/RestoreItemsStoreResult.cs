namespace GameTools.Server.Application.Abstractions.Stores.WriteStore
{
    public sealed record RestoreItemsStoreResult(
        Guid RestoreId,
        int Deleted,
        int Inserted,
        int Updated);
}
