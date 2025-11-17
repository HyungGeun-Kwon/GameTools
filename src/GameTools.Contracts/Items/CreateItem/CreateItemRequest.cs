namespace GameTools.Contracts.Items.CreateItem
{
    public sealed record CreateItemRequest(string Name, int Price, byte RarityId, string? Description = null);
}
