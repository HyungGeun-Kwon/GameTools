using GameTools.Server.Application.Abstractions.Users;
using GameTools.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.IntegrationTests.Fixtures
{
    public sealed class TestDbContextFactory(string connectionString)
    {
        public AppDbContext Create(string actor = "TestUser")
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString, sql =>
                {
                    // 마이그레이션 어셈블리 지정(Infra 프로젝트)
                    sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                })
                .EnableSensitiveDataLogging()
                .Options;

            return new AppDbContext(options, new FakeCurrentUser(actor));
        }

        private sealed class FakeCurrentUser(string user) : ICurrentUser
        {
            public string UserIdOrName { get; private set; } = user;

            public void Set(string user) => UserIdOrName = user;
        }
    }
}
