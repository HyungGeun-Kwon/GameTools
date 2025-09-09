namespace GameTools.Client.Application.UseCases.Items.BulkInsertItems
{
    public sealed record BulkInsertItemOutputRow(int? Id, string Status, string? RowVersionBase64);
}
