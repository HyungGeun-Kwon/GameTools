namespace GameTools.Client.Application.UseCases.Items.RestoreItemsAsOf
{
    public sealed record RestoreItemsAsOfOutput(Guid RestoreId, int Deleted, int Inserted, int Updated, bool IsChanged);
}
