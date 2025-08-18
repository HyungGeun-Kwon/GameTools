namespace GameTools.Application.Features.Items.Dtos
{
    public sealed record ItemUpdateDto(int Id, string Name, int Price, string? Description, byte RarityId, string RowVersionBase64);
}
