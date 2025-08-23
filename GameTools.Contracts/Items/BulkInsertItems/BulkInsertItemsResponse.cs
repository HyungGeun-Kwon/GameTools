namespace GameTools.Contracts.Items.InsertItemsTvp
{
    public sealed record BulkInsertItemsResponse(IReadOnlyList<BulkInsertItemResult> Results);
}
