using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Features.Items.Commands.Common.Bulk;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using GameTools.Server.Domain.Features.Items.Entities;
using GameTools.Server.Domain.Features.Items.ValueObjects;
using Microsoft.EntityFrameworkCore;

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

        public Task<IReadOnlyList<BulkResultRow>> BulkDeleteAsync(IReadOnlyList<DeleteItemSpec> items, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<BulkResultRow>> BulkInsertAsync(IReadOnlyList<CreateItemSpec> items, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<BulkResultRow>> BulkUpdateAsync(IReadOnlyList<UpdateItemSpec> items, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        // 세션 컨텍스트에 Actor 설정 (트리거 감사용)
        // 같은 커넥션에서 SP 호출 전에 매번 실행
        //private async Task SetSessionActorAsync(DbConnection conn, CancellationToken ct)
        //{
        //    await using var cmd = conn.CreateCommand();
        //    cmd.CommandText = "EXEC sys.sp_set_session_context @key=N'actor', @value=@p0";
        //    var p0 = cmd.CreateParameter();
        //    p0.ParameterName = "@p0";
        //    p0.Value = string.IsNullOrWhiteSpace(currentUser.UserIdOrName) ? "unknown" : currentUser.UserIdOrName;
        //    cmd.Parameters.Add(p0);
        //    await cmd.ExecuteNonQueryAsync(ct);
        //}
    }
}
