namespace GameTools.Client.Application.UseCases.Items.BulkInsertItems
{
    public sealed record BulkInsertItemInputRow(string Name, int Price, byte RarityId, string? Description);
}
