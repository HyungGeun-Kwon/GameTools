namespace GameTools.Server.Application.Features.Items.Models
{
    public sealed record ItemReadModel(
        Guid Id, string Name, int Price, string? Description,
        Guid RarityId, byte[] RowVersion
    );
}
