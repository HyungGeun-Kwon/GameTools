namespace GameTools.Application.Features.Items.Dtos
{
    public sealed record ItemDto(
        int Id,
        string Name,
        int Price,
        string? Description,
        byte RarityId,
        string RarityGrade,
        string RarityColorCode,
        string RowVersionBase64
    );
}
