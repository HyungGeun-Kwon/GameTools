namespace GameTools.Server.Application.Features.Items.Models
{
    public sealed record ItemReadModel(
        int Id, string Name, int Price, string? Description,
        byte RarityId, string RarityGrade, string RarityColorCode,
        byte[] RowVersion
    );
}
