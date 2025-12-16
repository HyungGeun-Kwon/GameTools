using System.Data;
using System.Data.Common;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.Users;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Bulk;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;
using GameTools.Server.Domain.Catalog.Items.Entities;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;
using GameTools.Server.Infrastructure.Persistence.Catalog.Tvp;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace GameTools.Server.Infrastructure.Persistence.Catalog.Stores.WriteStore
{
    public sealed class ItemWriteStore(AppDbContext db) : IItemWriteStore
    {
        public async Task AddAsync(Item aggregate, CancellationToken ct)
            => await db.Items.AddAsync(aggregate, ct);

        public void Remove(Item aggregate)
            => db.Items.Remove(aggregate);

        public byte[] GetRowVersion(Item aggregate)
        {
            var property = db.Entry(aggregate).Property<byte[]>("RowVersion");
            return property.CurrentValue ?? property.OriginalValue;
        }

        public Task<Item?> LoadForUpdateAsync(ItemId id, CancellationToken ct)
            => db.Items.SingleOrDefaultAsync(i => i.Id == id, ct);

        public void SetOriginalRowVersion(Item aggregate, byte[] rowVersion)
            => db.Entry(aggregate).Property<byte[]>("RowVersion").OriginalValue = rowVersion;

        public Task<IReadOnlyList<BulkResultRow>> BulkInsertAsync(IReadOnlyList<CreateItemSpec> items, CancellationToken ct)
            => ExecuteTvpAsync(
                storedProc: "dbo.usp_ItemBulkInsert",
                tvpTypeName: "dbo.ItemInsertTvp",
                tvpParamName: "@Items",
                tvp: ItemTvpTableFactory.CreateInsertTable(items),
                ct);

        public Task<IReadOnlyList<BulkResultRow>> BulkUpdateAsync(IReadOnlyList<UpdateItemSpec> items, CancellationToken ct)
            => ExecuteTvpAsync(
                storedProc: "dbo.usp_ItemBulkUpdate",
                tvpTypeName: "dbo.ItemUpdateTvp",
                tvpParamName: "@Items",
                tvp: ItemTvpTableFactory.CreateUpdateTable(items),
                ct);

        public Task<IReadOnlyList<BulkResultRow>> BulkDeleteAsync(IReadOnlyList<DeleteItemSpec> items, CancellationToken ct)
            => ExecuteTvpAsync(
                storedProc: "dbo.usp_ItemBulkDelete",
                tvpTypeName: "dbo.ItemDeleteTvp",
                tvpParamName: "@Items",
                tvp: ItemTvpTableFactory.CreateDeleteTable(items),
                ct);

        private Task<IReadOnlyList<BulkResultRow>> ExecuteTvpAsync(
            string storedProc,
            string tvpTypeName,
            string tvpParamName,
            DataTable tvp,
            CancellationToken ct)
        {
            return db.WithSessionActorAsync(async conn =>
            {
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = storedProc;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 60;

                var efTx = db.Database.CurrentTransaction;
                if (efTx != null)
                    cmd.Transaction = efTx.GetDbTransaction();

                var p = new SqlParameter(tvpParamName, SqlDbType.Structured)
                {
                    TypeName = tvpTypeName,
                    Value = tvp
                };
                cmd.Parameters.Add(p);

                await using var reader = await cmd.ExecuteReaderAsync(ct);
                return await ReadBulkResultRowsAsync(reader, ct);
            }, ct);
        }

        private static async Task<IReadOnlyList<BulkResultRow>> ReadBulkResultRowsAsync(DbDataReader reader, CancellationToken ct)
        {
            var results = new List<BulkResultRow>();

            while (await reader.ReadAsync(ct))
            {
                var index = reader.GetInt32(0);
                var id = reader.GetGuid(1);
                byte[]? rowVersion = await reader.IsDBNullAsync(2, ct) ? null : (byte[])reader.GetValue(2);
                var status = (BulkStatusCode)reader.GetByte(3);
                string? errorCode = await reader.IsDBNullAsync(4, ct) ? null : reader.GetString(4);
                string? errorMessage = await reader.IsDBNullAsync(5, ct) ? null : reader.GetString(5);

                results.Add(new BulkResultRow(index, id, rowVersion, status, errorCode, errorMessage));
            }

            return results;
        }
    }

    internal static class EfTxExtensions
    {
        public static DbTransaction GetDbTransaction(this IDbContextTransaction tx)
            => tx.GetDbTransaction();
    }
}