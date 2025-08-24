namespace GameTools.Contracts.Items.BulkInsertItems
{
    public sealed record BulkInsertItemResult(int Id, string RowVersionBase64);
}
