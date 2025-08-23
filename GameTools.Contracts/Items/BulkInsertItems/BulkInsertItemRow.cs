namespace GameTools.Contracts.Items
{
    public sealed record BulkInsertItemRow(string Name, int Price, byte RarityId, string? Description);
}
