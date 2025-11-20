namespace GameTools.Server.Application.Features.Items.Commands.UpdateItem
{
    public sealed record UpdateItemResult(
        Guid Id,
        byte[] RowVersion);
}
