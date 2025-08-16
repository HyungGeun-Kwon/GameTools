using System.Data;
using GameTools.Application.Abstractions.Users;
using GameTools.Application.Abstractions.Works;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Infrastructure.Persistence.Works
{
    public sealed class UnitOfWork(AppDbContext db, ICurrentUser currentUser) : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => db.SaveChangesAsync(ct);

        public async Task<int> SaveChangesAsync2(CancellationToken ct = default)
        {
            await using var conn = db.Database.GetDbConnection(); if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct); // SaveChanges 전에 Actor 설정
            await using (var cmd = conn.CreateCommand()) { cmd.CommandText = "EXEC sys.sp_set_session_context @key=N'actor', @value=@p0"; var p0 = cmd.CreateParameter(); p0.ParameterName = "@p0"; p0.Value = currentUser.UserIdOrName ?? "system"; cmd.Parameters.Add(p0); await cmd.ExecuteNonQueryAsync(ct); } return await db.SaveChangesAsync(ct); }
        }
}
