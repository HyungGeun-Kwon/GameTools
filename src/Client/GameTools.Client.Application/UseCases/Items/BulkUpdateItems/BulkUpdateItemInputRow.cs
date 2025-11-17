namespace GameTools.Client.Application.UseCases.Items.BulkUpdateItems
{
    public sealed record BulkUpdateItemInputRow(int Id, string Name, int Price, string? Description, byte RarityId, string RowVersionBase64);
}
