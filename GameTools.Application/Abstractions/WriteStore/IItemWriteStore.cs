using GameTools.Application.Features.Items.Commands.Common;
using GameTools.Domain.Entities;

namespace GameTools.Application.Abstractions.WriteStore
{
    public interface IItemWriteStore : IWriteStore<Item, int>
    {
        Task<IReadOnlyList<(int Id, byte[] NewRowVersion)>> InsertManyTvpAsync(
            IEnumerable<ItemInsertRow> rows, CancellationToken ct);

        // StatusCode: 0=Updated, 1=NotFound, 2=Concurrency
        Task<IReadOnlyList<(int Id, byte[]? NewRowVersion, UpdateStatusCode StatusCode)>> UpdateManyTvpAsync(
            IEnumerable<ItemUpdateRow> rows, CancellationToken ct);
    }
}
