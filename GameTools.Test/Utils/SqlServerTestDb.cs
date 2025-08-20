using GameTools.Domain.Entities;
using GameTools.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Test.Utils
{
    public sealed class SqlServerTestDb : IAsyncDisposable
    {
        public AppDbContext Db { get; }
        private readonly string _cs;

        private SqlServerTestDb(AppDbContext db, string cs) { Db = db; _cs = cs; }

        public static async Task<SqlServerTestDb> CreateAsync(bool migrate = true)
        {
            var dbName = "gt_tests_" + Guid.NewGuid().ToString("N");
            var cs = $"Server=(localdb)\\MSSQLLocalDB;Initial Catalog={dbName};" +
                     $"Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True";

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(cs)
                .EnableSensitiveDataLogging()
                .Options;

            var db = new AppDbContext(options);
            if (migrate) await db.Database.MigrateAsync();
            else await db.Database.EnsureCreatedAsync();

            return new SqlServerTestDb(db, cs);
        }
        
        public Rarity SeedRarity(string grade = "Common", string colorCode = "#000000")
        {
            var rarity = new Rarity(grade, colorCode);
            Db.Set<Rarity>().Add(rarity);
            Db.SaveChanges();
            return rarity;
        }

        public AppDbContext NewDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(_cs)
                .EnableSensitiveDataLogging()
                .Options;
            return new AppDbContext(options);
        }

        public async ValueTask DisposeAsync()
        {
            // DB/파일까지 삭제 → 흔적 안 남음
            await Db.Database.EnsureDeletedAsync();
            await Db.DisposeAsync();
        }
    }
}
