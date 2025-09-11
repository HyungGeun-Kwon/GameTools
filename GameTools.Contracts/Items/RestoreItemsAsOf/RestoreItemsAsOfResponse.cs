namespace GameTools.Contracts.Items.RestoreItemsAsOf
{
    public sealed record RestoreItemsAsOfResponse(Guid RestoreId, int Deleted, int Inserted, int Updated, bool IsChanged);
}
