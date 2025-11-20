namespace GameTools.Server.Application.Features.Restores.Commands.RestoreItems
{
    public sealed record RestoreItemsResult(Guid RestoreId, int Deleted, int Inserted, int Updated, bool IsChanged);
}
