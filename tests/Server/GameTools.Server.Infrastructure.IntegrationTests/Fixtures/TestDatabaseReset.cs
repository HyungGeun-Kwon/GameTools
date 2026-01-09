using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTools.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.IntegrationTests.Fixtures
{
    internal static class TestDatabaseReset
    {
        public static async Task ResetAsync(AppDbContext db, CancellationToken ct = default)
        {
            await db.Database.ExecuteSqlRawAsync("DELETE FROM dbo.ItemAudit;", ct);
            await db.Database.ExecuteSqlRawAsync("DELETE FROM dbo.RarityAudit;", ct);
            await db.Database.ExecuteSqlRawAsync("DELETE FROM dbo.RestoreHistory;", ct);

            await db.Database.ExecuteSqlRawAsync("DELETE FROM dbo.Item;", ct);
            await db.Database.ExecuteSqlRawAsync("DELETE FROM dbo.Rarity;", ct);

            db.ChangeTracker.Clear();
        }
    }
}
