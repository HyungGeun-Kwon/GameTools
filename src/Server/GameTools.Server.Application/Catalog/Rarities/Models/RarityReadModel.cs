namespace GameTools.Server.Application.Catalog.Rarities.Models
{
    public sealed record RarityReadModel(Guid Id, string Grade, string ColorCode, byte[] RowVersion);
}
