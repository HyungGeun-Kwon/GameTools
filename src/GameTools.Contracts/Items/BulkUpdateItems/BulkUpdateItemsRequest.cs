namespace GameTools.Contracts.Items.BulkUpdateItems
{
    public sealed record BulkUpdateItemsRequest(IReadOnlyList<BulkUpdateItemRow> BulkUpdateItemRows);
}
