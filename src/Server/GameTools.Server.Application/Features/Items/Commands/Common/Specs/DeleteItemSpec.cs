namespace GameTools.Server.Application.Features.Items.Commands.Common.Specs
{
    public sealed record DeleteItemSpec(Guid Id, byte[] RowVersion);
}
