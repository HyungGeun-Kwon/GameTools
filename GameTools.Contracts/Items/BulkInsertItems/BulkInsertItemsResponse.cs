namespace GameTools.Contracts.Items.BulkInsertItems
{
    public sealed record BulkInsertItemsResponse(IReadOnlyList<BulkInsertItemResult> Results);
}
