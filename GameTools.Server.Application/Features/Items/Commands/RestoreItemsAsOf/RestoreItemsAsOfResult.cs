namespace GameTools.Server.Application.Features.Items.Commands.RestoreItemsAsOf
{
    public sealed record RestoreItemsAsOfResult(Guid RestoreId, int Deleted, int Inserted, int Updated, bool IsChanged);
}
