namespace GameTools.Server.Application.Operations.Restores.Commands.RestoreItems
{
    public sealed record RestoreItemsResult(Guid RestoreId, int Deleted, int Inserted, int Updated, bool IsChanged);
}
