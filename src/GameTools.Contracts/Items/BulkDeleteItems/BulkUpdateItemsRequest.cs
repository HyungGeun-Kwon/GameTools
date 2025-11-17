namespace GameTools.Contracts.Items.BulkDeleteItems
{
    public sealed record BulkDeleteItemsRequest(IReadOnlyList<BulkDeleteItemRow> BulkDeleteItemRows);
}
