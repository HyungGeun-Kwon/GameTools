namespace GameTools.Client.Application.UseCases.Items.BulkUpdateItems
{
    public sealed record BulkUpdateItemOutputRow(int Id, string Status, string? NewRowVersionBase64);
}
