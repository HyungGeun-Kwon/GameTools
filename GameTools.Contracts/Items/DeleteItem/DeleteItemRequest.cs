namespace GameTools.Contracts.Items.DeleteItem
{
    public sealed record DeleteItemRequest(int Id, string RowVersionBase64);
}
