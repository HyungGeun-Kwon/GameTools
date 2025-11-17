namespace GameTools.Client.Application.UseCases.Items.UpdateItem
{
    public sealed record UpdateItemInput(
        int Id, string Name, int Price, string? Description,
        byte RarityId, string RowVersionBase64);
}
