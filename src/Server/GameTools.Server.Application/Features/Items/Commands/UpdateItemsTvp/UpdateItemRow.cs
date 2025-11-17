namespace GameTools.Server.Application.Features.Items.Commands.UpdateItemsTvp
{
    public sealed record UpdateItemRow(int Id, string Name, int Price, string? Description, byte RarityId, byte[] RowVersion);
}
