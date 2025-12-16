namespace GameTools.Server.Application.Catalog.Items.Commands.UpdateItem
{
    public sealed record UpdateItemResult(
        Guid Id,
        byte[] RowVersion);
}
