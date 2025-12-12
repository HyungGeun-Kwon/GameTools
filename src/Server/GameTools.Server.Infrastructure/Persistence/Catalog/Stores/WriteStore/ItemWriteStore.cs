using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.Users;
using GameTools.Server.Application.Features.Items.Commands.Common.Bulk;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using GameTools.Server.Domain.Features.Items.Entities;
using GameTools.Server.Domain.Features.Items.ValueObjects;
using GameTools.Server.Infrastructure.Persistence.Catalog.Tvp;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;

namespace GameTools.Server.Infrastructure.Persistence.Catalog.Stores.WriteStore
{
    public sealed class ItemWriteStore(AppDbContext db, ICurrentUser currentUser) : IItemWriteStore
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

        private async Task<IReadOnlyList<BulkResultRow>> ExecuteTvpAsync(
            string storedProc,
            string tvpTypeName,
            string tvpParamName,
            DataTable tvp,
            CancellationToken ct)
        {
            var conn = db.Database.GetDbConnection();

            var openedHere = false;
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync(ct);
                openedHere = true;
            }

            try
            {
                await SetSessionCurrentUserAsync(conn, ct);

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = storedProc;
                cmd.CommandType = CommandType.StoredProcedure;

                var p = new SqlParameter(tvpParamName, SqlDbType.Structured)
                {
                    TypeName = tvpTypeName,
                    Value = tvp
                };
                cmd.Parameters.Add(p);

                await using var reader = await cmd.ExecuteReaderAsync(ct);
                return await ReadBulkResultRowsAsync(reader, ct);
            }
            finally
            {
                if (openedHere) await conn.CloseAsync();
            }
        }

        private static async Task<IReadOnlyList<BulkResultRow>> ReadBulkResultRowsAsync(DbDataReader reader, CancellationToken ct)
        {
            var results = new List<BulkResultRow>();

            // SP 결과 컬럼:
            // 0 Index(int), 1 Id(uniqueidentifier), 2 RowVersion(varbinary(8) null),
            // 3 Status(tinyint), 4 ErrorCode(nvarchar null), 5 ErrorMessage(nvarchar null)
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

        private async Task SetSessionCurrentUserAsync(DbConnection conn, CancellationToken ct)
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "EXEC sys.sp_set_session_context @key=N'CurrentUser', @value=@p0";

            var p0 = cmd.CreateParameter();
            p0.ParameterName = "@p0";
            p0.Value = string.IsNullOrWhiteSpace(currentUser.UserIdOrName) ? "unknown" : currentUser.UserIdOrName;

            cmd.Parameters.Add(p0);

            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}
