namespace GameTools.Server.Infrastructure.Persistence.Catalog.Seed
{
    public interface ISeeder
    {
        Task SeedAsync(CancellationToken ct = default);
    }
}
