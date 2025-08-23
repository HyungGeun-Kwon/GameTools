using GameTools.Server.Application.Features.Items.Commands.InsertItemsTvp;
using GameTools.Server.Application.Features.Items.Commands.UpdateItemsTvp;
using GameTools.Server.Domain.Entities;

namespace GameTools.Server.Application.Abstractions.Stores.WriteStore
{
    public interface IItemWriteStore : IWriteStore<Item, int>
    {
        Task<IReadOnlyList<(int Id, byte[] NewRowVersion)>> InsertManyTvpAsync(
            IEnumerable<InsertItemRow> rows, CancellationToken ct);

        // StatusCode: 0=Updated, 1=NotFound, 2=Concurrency
        Task<IReadOnlyList<(int Id, byte[]? NewRowVersion, UpdateStatusCode StatusCode)>> UpdateManyTvpAsync(
            IEnumerable<UpdateItemRow> rows, CancellationToken ct);

        void SetOriginalRowVersion(Item entity, byte[] rowVersion);
    }
}
