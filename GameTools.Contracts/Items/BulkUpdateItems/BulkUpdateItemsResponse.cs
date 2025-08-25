namespace GameTools.Contracts.Items.BulkUpdateItems
{
    public sealed record BulkUpdateItemsResponse(IReadOnlyList<BulkUpdateItemResult> Results);
}
