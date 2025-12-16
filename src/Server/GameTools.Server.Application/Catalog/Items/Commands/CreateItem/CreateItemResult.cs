namespace GameTools.Server.Application.Catalog.Items.Commands.CreateItem
{
    public sealed record CreateItemResult(
        Guid Id,
        byte[] RowVersion
    );
}
