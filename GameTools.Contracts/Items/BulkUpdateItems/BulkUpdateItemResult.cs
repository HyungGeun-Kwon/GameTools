namespace GameTools.Contracts.Items.BulkUpdateItems
{
    public sealed record BulkUpdateItemResult(int Id, string Status, string? NewRowVersionBase64);
}
