using GameTools.Server.Application.Features.Items.Commands.DeleteItemsTvp;
using GameTools.Server.Application.Features.Items.Commands.InsertItemsTvp;
using GameTools.Server.Application.Features.Items.Commands.UpdateItemsTvp;
using GameTools.Server.Domain.Entities;

namespace GameTools.Server.Application.Abstractions.Stores.WriteStore
{
    public interface IItemWriteStore : IWriteStore<Item, int>
    {
        Task<IReadOnlyList<(int? Id, byte[]? NewRowVersion, BulkInsertStatusCode StatusCode)>> InsertManyTvpAsync(
            IEnumerable<InsertItemRow> rows, CancellationToken ct);

        // StatusCode: 0=Updated, 1=NotFound, 2=Concurrency
        Task<IReadOnlyList<(int Id, byte[]? NewRowVersion, BulkUpdateStatusCode StatusCode)>> UpdateManyTvpAsync(
            IEnumerable<UpdateItemRow> rows, CancellationToken ct);

        Task<IReadOnlyList<(int? Id, BulkDeleteStatusCode StatusCode)>> DeleteManyTvpAsync(
            IEnumerable<DeleteItemRow> rows, CancellationToken ct);

        void SetOriginalRowVersion(Item entity, byte[] rowVersion);
    }
}
