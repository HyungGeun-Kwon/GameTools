namespace GameTools.Contracts.Items.BulkInsertItems
{
    public sealed record BulkInsertItemResult(int? Id, string Status, string? RowVersionBase64);
}
