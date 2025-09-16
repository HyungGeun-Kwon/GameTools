namespace GameTools.Client.Application.UseCases.Items.DeleteItem
{
    public sealed record DeleteItemInput(int Id, string RowVersionBase64);
}
