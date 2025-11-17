namespace GameTools.Contracts.Items.BulkInsertItems
{
    public sealed record BulkInsertItemRow(string Name, int Price, byte RarityId, string? Description);
}
