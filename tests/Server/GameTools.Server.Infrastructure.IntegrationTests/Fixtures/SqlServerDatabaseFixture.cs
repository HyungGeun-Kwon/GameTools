using GameTools.Server.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.IntegrationTests.Fixtures
{
    public sealed class SqlServerDatabaseFixture : IAsyncLifetime
    {
        private readonly string _serverConnectionString;
        private bool _migrated;

        public string DatabaseName { get; } = $"GameDb_It_{Guid.NewGuid():N}";
        public string TestDatabaseConnectionString { get; private set; } = default!;

        public SqlServerDatabaseFixture()
        {
            _serverConnectionString =
                Environment.GetEnvironmentVariable("GT_SQLSERVER_TEST_SERVER_CS")
                ?? "Server=localhost;Trusted_Connection=True;TrustServerCertificate=True;";

            // 테스트 DB로 붙을 연결 문자열
            var csb = new SqlConnectionStringBuilder(_serverConnectionString)
            {
                InitialCatalog = DatabaseName
            };
            TestDatabaseConnectionString = csb.ConnectionString;
        }

        public async Task InitializeAsync()
        {
            await CreateDatabaseAsync();
        }

        public async Task DisposeAsync()
        {
            await DropDatabaseAsync();
        }
        public async Task EnsureMigratedAsync(Func<AppDbContext> createDb)
        {
            if (_migrated) return;

            await using var db = createDb();
            await db.Database.MigrateAsync();
            _migrated = true;
        }
        private async Task CreateDatabaseAsync()
        {
            await using var conn = new SqlConnection(_serverConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"
                IF DB_ID(N'{DatabaseName}') IS NULL
                BEGIN
                    CREATE DATABASE [{DatabaseName}];
                END";
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task DropDatabaseAsync()
        {
            await using var conn = new SqlConnection(_serverConnectionString);
            await conn.OpenAsync();

            // 연결 끊고 drop (강제)
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"
                IF DB_ID(N'{DatabaseName}') IS NOT NULL
                BEGIN
                    ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{DatabaseName}];
                END";
            await cmd.ExecuteNonQueryAsync();
        }
    }

    [CollectionDefinition(Name)]
    public sealed class SqlServerDatabaseCollection : ICollectionFixture<SqlServerDatabaseFixture>
    {
        public const string Name = "SqlServerDatabaseCollection";
    }
}
