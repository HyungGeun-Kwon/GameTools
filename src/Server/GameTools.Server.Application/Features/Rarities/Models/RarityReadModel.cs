namespace GameTools.Server.Application.Features.Rarities.Models
{
    public sealed record RarityReadModel(Guid Id, string Grade, string ColorCode, byte[] RowVersion);
}
