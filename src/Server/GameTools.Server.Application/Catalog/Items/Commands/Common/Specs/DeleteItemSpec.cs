namespace GameTools.Server.Application.Catalog.Items.Commands.Common.Specs
{
    public sealed record DeleteItemSpec(Guid Id, byte[] RowVersion);
}
