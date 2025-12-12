using GameTools.Server.Application.Abstractions.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GameTools.Server.Infrastructure.Persistence
{
    public sealed class DesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public class FakeCurrentUser : ICurrentUser
        {
            public string UserIdOrName { get; private set; } = "DesignTime";

            public void Set(string user)
            {
                UserIdOrName = user;
            }
        }
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

            var fakeCurrentUser = new FakeCurrentUser();

            return new AppDbContext(opts, fakeCurrentUser);
        }
    }
}
