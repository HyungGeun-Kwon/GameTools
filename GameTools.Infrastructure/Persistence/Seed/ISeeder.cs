namespace GameTools.Infrastructure.Persistence.Seed
{
    public interface ISeeder
    {
        Task SeedAsync(AppDbContext dbContext, CancellationToken ct = default);
    }
}
