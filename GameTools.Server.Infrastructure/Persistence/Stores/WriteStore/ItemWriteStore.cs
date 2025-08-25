using System.Data;
using System.Data.Common;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.Users;
using GameTools.Server.Application.Features.Items.Commands.InsertItemsTvp;
using GameTools.Server.Application.Features.Items.Commands.UpdateItemsTvp;
using GameTools.Server.Domain.Entities;
using GameTools.Server.Infrastructure.Persistence.Tvp;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence.Stores.WriteStore
{
    public sealed class ItemWriteStore(AppDbContext db, ICurrentUser currentUser) : IItemWriteStore
    {
        public async Task<Item?> GetByIdAsync(int id, CancellationToken ct)
            => await db.Items
                .Include(i => i.Rarity)
                .SingleOrDefaultAsync(i => i.Id == id, ct);
        public async Task AddAsync(Item entity, CancellationToken ct)
            => await db.Items.AddAsync(entity, ct);

        public void Remove(Item entity)
            => db.Items.Remove(entity);

        public void SetOriginalRowVersion(Item entity, byte[] rowVersion)
        {
            db.Entry(entity).Property(e => e.RowVersion).OriginalValue = rowVersion;
        }

        public async Task<IReadOnlyList<(int Id, byte[] NewRowVersion)>> InsertManyTvpAsync(
            IEnumerable<InsertItemRow> rows, CancellationToken ct)
        {
            var table = TvpTableFactory.CreateItemInsertDataTable(rows);

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
                cmd.CommandText = TvpNames.ItemInsertProc;
                cmd.CommandType = CommandType.StoredProcedure;

                var p = new SqlParameter("@Rows", SqlDbType.Structured)
                {
                    TypeName = TvpNames.ItemInsertType,
                    Value = table
                };
                cmd.Parameters.Add(p);

                var result = new List<(int, byte[])>();
                await using var reader = await cmd.ExecuteReaderAsync(ct);
                while (await reader.ReadAsync(ct))
                {
                    var id = reader.GetInt32(0);
                    var rv = (byte[])reader.GetValue(1); // varbinary(8)
                    result.Add((id, rv));
                }
                return result;
            }
            finally
            {
                if (openedHere) await conn.CloseAsync();
            }
        }

        public async Task<IReadOnlyList<(int Id, byte[]? NewRowVersion, BulkUpdateStatusCode StatusCode)>> UpdateManyTvpAsync(
            IEnumerable<UpdateItemRow> rows, CancellationToken ct)
        {
            var table = TvpTableFactory.CreateItemUpdateDataTable(rows);

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
                cmd.CommandText = TvpNames.ItemUpdateProc;
                cmd.CommandType = CommandType.StoredProcedure;

                var p = new SqlParameter("@Rows", SqlDbType.Structured)
                {
                    TypeName = TvpNames.ItemUpdateType,
                    Value = table
                };
                cmd.Parameters.Add(p);

                var result = new List<(int, byte[]?, BulkUpdateStatusCode)>();
                await using var reader = await cmd.ExecuteReaderAsync(ct);
                while (await reader.ReadAsync(ct))
                {
                    var id = reader.GetInt32(0);
                    var isNull = await reader.IsDBNullAsync(1, ct);
                    var newRv = isNull ? null : (byte[])reader.GetValue(1);
                    var statusCode = (BulkUpdateStatusCode)reader.GetInt32(2);

                    result.Add((id, newRv, statusCode));
                }
                return result;
            }
            finally
            {
                if (openedHere) await conn.CloseAsync();
            }
        }

        // 세션 컨텍스트에 Actor 설정 (트리거 감사용)
        // 같은 커넥션에서 SP 호출 전에 매번 실행
        private async Task SetSessionActorAsync(DbConnection conn, CancellationToken ct)
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "EXEC sys.sp_set_session_context @key=N'actor', @value=@p0";
            var p0 = cmd.CreateParameter();
            p0.ParameterName = "@p0";
            p0.Value = string.IsNullOrWhiteSpace(currentUser.UserIdOrName) ? "unknown" : currentUser.UserIdOrName;
            cmd.Parameters.Add(p0);
            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}
