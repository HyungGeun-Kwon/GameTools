namespace GameTools.Contracts.Items.UpdateItem
{
    public sealed record UpdateItemRequest(int Id, string Name, int Price, string? Description, byte RarityId, string RowVersionBase64);
}
