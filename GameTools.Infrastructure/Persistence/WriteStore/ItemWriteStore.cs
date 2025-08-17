using System.Data;
using System.Data.Common;
using GameTools.Application.Abstractions.Users;
using GameTools.Application.Abstractions.WriteStore;
using GameTools.Application.Features.Items.Commands.Common;
using GameTools.Domain.Entities;
using GameTools.Infrastructure.Persistence.Tvp;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Infrastructure.Persistence.WriteStore
{
    public sealed class ItemWriteStore(AppDbContext db, ICurrentUser currentUser) : IItemWriteStore
    {
        public async Task<Item?> GetByIdAsync(int id, CancellationToken ct)
            => await db.Items.FindAsync([id], ct);

        public async Task AddAsync(Item entity, CancellationToken ct)
            => await db.Items.AddAsync(entity, ct);

        public void Remove(Item entity)
            => db.Items.Remove(entity);

        public async Task<IReadOnlyList<(int Id, byte[] NewRowVersion)>> InsertManyTvpAsync(
            IEnumerable<ItemInsertRow> rows, CancellationToken ct)
        {
            var table = TvpTableFactory.CreateItemInsertDataTable(rows);

            await using var conn = db.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct);

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

        public async Task<IReadOnlyList<(int Id, byte[]? NewRowVersion, UpdateStatusCode StatusCode)>> UpdateManyTvpAsync(
            IEnumerable<ItemUpdateRow> rows, CancellationToken ct)
        {
            var table = TvpTableFactory.CreateItemUpdateDataTable(rows);

            await using var conn = db.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync(ct);

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

            var result = new List<(int, byte[]?, UpdateStatusCode)>();
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                var id = reader.GetInt32(0);
                var isNull = await reader.IsDBNullAsync(1, ct);
                var newRv = isNull ? null : (byte[])reader.GetValue(1);
                var statusCode = (UpdateStatusCode)reader.GetByte(2);

                result.Add((id, newRv, statusCode));
            }
            return result;
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
