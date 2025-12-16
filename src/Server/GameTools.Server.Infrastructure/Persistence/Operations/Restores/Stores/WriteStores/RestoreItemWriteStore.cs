using System.Data;
using System.Data.Common;
using System.Text.Json;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Operations.Restores.Commands.RestoreItems;
using GameTools.Server.Infrastructure.Persistence.Catalog.Stores.WriteStore;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence.Operations.Restores.Stores.WriteStores
{
    public sealed class RestoreItemWriteStore(AppDbContext db) : IRestoreItemWriteStore
    {
        public async Task<RestoreItemsStoreResult> RestoreItemsAsOfAsync(RestoreItemsSpec spec, CancellationToken ct)
        {
            if (!db.Database.IsRelational())
                throw new NotSupportedException("RestoreItemsAsOf is only supported for relational databases.");

            var itemIdsJson = spec.ItemIds is null ? null : JsonSerializer.Serialize(spec.ItemIds);

            return await db.WithSessionActorAsync(async conn =>
            {
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "dbo.usp_ItemRestore_AsOf";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 60;

                var efTx = db.Database.CurrentTransaction;
                if (efTx != null)
                    cmd.Transaction = efTx.GetDbTransaction();

                AddParam(cmd, "@AsOfUtc", DbType.DateTime2, spec.AsOfUtc);
                AddParam(cmd, "@ItemIdsJson", DbType.String, (object?)itemIdsJson ?? DBNull.Value);
                AddParam(cmd, "@DryRun", DbType.Boolean, spec.DryRun);
                AddParam(cmd, "@Notes", DbType.String, (object?)spec.Notes ?? DBNull.Value);

                await using var reader = await cmd.ExecuteReaderAsync(ct);

                if (!await reader.ReadAsync(ct))
                    throw new InvalidOperationException("Restore proc returned no result set.");

                var restoreId = reader.GetGuid(0);
                var deleted = reader.GetInt32(1);
                var inserted = reader.GetInt32(2);
                var updated = reader.GetInt32(3);
                var isChanged = reader.GetBoolean(4);

                return new RestoreItemsStoreResult(restoreId, deleted, inserted, updated, isChanged);
            }, ct);
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