namespace GameTools.Server.Application.Features.Items.Commands.DeleteItem
{
    public sealed record DeleteItemPayload(int Id, byte[] RowVersion);
}
