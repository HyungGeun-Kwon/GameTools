using System.Data;
using GameTools.Application.Abstractions.Users;
using GameTools.Application.Abstractions.Works;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Infrastructure.Persistence.Works
{
    public sealed class UnitOfWork(AppDbContext db, ICurrentUser currentUser) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            if (!db.Database.IsRelational())
                return await db.SaveChangesAsync(ct); // InMemory일 경우.

            var conn = db.Database.GetDbConnection();

            var openedHere = false;
            if (conn.State != ConnectionState.Open)
            {
                await db.Database.OpenConnectionAsync(ct);
                openedHere = true;
            }

            try
            {
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "EXEC sys.sp_set_session_context @key=N'actor', @value=@p0";
                var p0 = cmd.CreateParameter();
                p0.ParameterName = "@p0";
                p0.Value = currentUser.UserIdOrName ?? "unknown";
                cmd.Parameters.Add(p0);
                await cmd.ExecuteNonQueryAsync(ct);

                return await db.SaveChangesAsync(ct);
            }
            finally { if (openedHere) await db.Database.CloseConnectionAsync(); }
        }
    }
}