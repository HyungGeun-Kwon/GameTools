namespace GameTools.Client.Application.UseCases.Items.BulkInsertItems
{
    public sealed record BulkInsertItemInputRow(string Name, int Price, string? Description, byte RarityId);
}
