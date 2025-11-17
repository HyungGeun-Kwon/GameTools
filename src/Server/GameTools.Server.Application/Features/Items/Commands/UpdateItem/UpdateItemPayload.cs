namespace GameTools.Server.Application.Features.Items.Commands.UpdateItem
{
    public sealed record UpdateItemPayload(int Id, string Name, int Price, string? Description, byte RarityId, byte[] RowVersion);
}
