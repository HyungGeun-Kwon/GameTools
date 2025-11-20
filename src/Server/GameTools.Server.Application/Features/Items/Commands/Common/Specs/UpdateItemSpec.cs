namespace GameTools.Server.Application.Features.Items.Commands.Common.Specs
{
    public sealed record UpdateItemSpec(
        Guid Id,
        string Name,
        int Price,
        string? Description,
        Guid RarityId,
        byte[] RowVersion);
}
