namespace GameTools.Contracts.Items.BulkInsertItems
{
    public sealed record BulkInsertItemsRequest(IReadOnlyList<BulkInsertItemRow> BulkInsertItems);
}
