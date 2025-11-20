namespace GameTools.Server.Application.Features.Items.Commands.CreateItem
{
    public sealed record CreateItemResult(
        Guid Id,
        byte[] RowVersion
    );
}
