namespace GameTools.Application.Features.Items.Commands.UpdateItemsTvp
{
    public sealed record UpdateItemRowRequest(int Id, string Name, int Price, string? Description, byte RarityId, string RowVersionBase64);
}
