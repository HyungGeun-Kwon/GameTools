namespace GameTools.Contracts.Items.BulkDeleteItems
{
    public sealed record BulkDeleteItemsResponse(IReadOnlyList<BulkDeleteItemResult> Results);
}
