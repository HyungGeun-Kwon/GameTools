namespace GameTools.Application.Features.Items.Dtos
{
    public sealed record ItemCreateDto(string Name, int Price, byte RarityId, string? Description = null);
}
