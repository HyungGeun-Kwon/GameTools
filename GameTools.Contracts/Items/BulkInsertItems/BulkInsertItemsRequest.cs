namespace GameTools.Contracts.Items.InsertItemsTvp
{
    public sealed record BulkInsertItemsRequest(IReadOnlyList<BulkInsertItemRow> BulkInsertItems);
}
