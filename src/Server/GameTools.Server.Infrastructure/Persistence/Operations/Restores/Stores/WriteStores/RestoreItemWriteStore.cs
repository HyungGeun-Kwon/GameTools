using System.Data;
using System.Data.Common;
using System.Text.Json;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.Users;
using GameTools.Server.Application.Features.Restores.Commands.RestoreItems;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence.Operations.Restores.Stores.WriteStores
{
    public sealed class RestoreItemWriteStore(AppDbContext db, ICurrentUser currentUser) : IRestoreItemWriteStore
    {
        public async Task<RestoreItemsStoreResult> RestoreItemsAsOfAsync(RestoreItemsSpec spec, CancellationToken ct)
        {
            if (!db.Database.IsRelational())
                throw new NotSupportedException("RestoreItemsAsOf is only supported for relational databases.");

            var itemIdsJson = spec.ItemIds is null ? null : JsonSerializer.Serialize(spec.ItemIds);

            var conn = db.Database.GetDbConnection();
            var openedHere = false;

            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync(ct);
                openedHere = true;
            }

            try
            {
                await SetSessionActorAsync(conn, ct);

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "dbo.usp_ItemRestore_AsOf";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 60;

                AddParam(cmd, "@AsOfUtc", DbType.DateTime2, spec.AsOfUtc);
                AddParam(cmd, "@ItemIdsJson", DbType.String, (object?)itemIdsJson ?? DBNull.Value);
                AddParam(cmd, "@DryRun", DbType.Boolean, spec.DryRun);
                AddParam(cmd, "@Notes", DbType.String, (object?)spec.Notes ?? DBNull.Value);

                await using var reader = await cmd.ExecuteReaderAsync(ct);

                // 결과 1셋: RestoreId, Deleted, Inserted, Updated, IsChanged
                if (!await reader.ReadAsync(ct))
                    throw new InvalidOperationException("Restore proc returned no result set.");

                var restoreId = reader.GetGuid(0);
                var deleted = reader.GetInt32(1);
                var inserted = reader.GetInt32(2);
                var updated = reader.GetInt32(3);
                var isChanged = reader.GetBoolean(4);

                return new RestoreItemsStoreResult(restoreId, deleted, inserted, updated, isChanged);
            }
            finally
            {
                // 커넥션 풀 재사용 대비: actor 키 제거
                try { await ClearSessionActorAsync(conn, ct); } catch { }

                if (openedHere)
                    await conn.CloseAsync();
            }
        }

        private async Task SetSessionActorAsync(DbConnection conn, CancellationToken ct)
        {
            var actor = currentUser.UserIdOrName ?? "unknown";

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "EXEC sys.sp_set_session_context @key=N'actor', @value=@p0";
            cmd.CommandType = CommandType.Text;

            var p0 = cmd.CreateParameter();
            p0.ParameterName = "@p0";
            p0.Value = actor;
            cmd.Parameters.Add(p0);

            await cmd.ExecuteNonQueryAsync(ct);
        }

        private async Task ClearSessionActorAsync(DbConnection conn, CancellationToken ct)
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "EXEC sys.sp_set_session_context @key=N'actor', @value=NULL";
            cmd.CommandType = CommandType.Text;
            await cmd.ExecuteNonQueryAsync(ct);
        }

        private static void AddParam(DbCommand cmd, string name, DbType dbType, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.DbType = dbType;
            p.Value = value;
            cmd.Parameters.Add(p);
        }
    }
}
