namespace GameTools.Application.Features.Rarities.Dtos
{
    public sealed record RarityUpdateDto(byte Id, string Grade, string ColorCode, string RowVersionBase64);
}
