namespace GameTools.Client.Application.UseCases.Items.BulkDeleteItems
{
    public sealed record BulkDeleteItemInputRow(int Id, string RowVersionBase64);
}
