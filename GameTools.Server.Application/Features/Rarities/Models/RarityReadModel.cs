namespace GameTools.Server.Application.Features.Rarities.Models
{
    public sealed record RarityReadModel(byte Id, string Grade, string ColorCode, byte[] RowVersion);
}
