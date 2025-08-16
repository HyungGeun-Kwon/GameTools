using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GameTools.Infrastructure.Persistence
{
    public sealed class DesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var cfg = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var cs = cfg.GetConnectionString("Default")
                     ?? "Server=localhost;Database=GameDb;Trusted_Connection=True;TrustServerCertificate=True";

            var opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(cs, sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
                .Options;

            return new AppDbContext(opts);
        }
    }
}
