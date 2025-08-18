using GameTools.Application.Features.Items.Commands.Common;
using GameTools.Application.Features.Items.Dtos;
using GameTools.Domain.Entities;

namespace GameTools.Application.Abstractions.WriteStore
{
    public interface IItemWriteStore : IWriteStore<Item, int>
    {
        Task<IReadOnlyList<(int Id, byte[] NewRowVersion)>> InsertManyTvpAsync(
            IEnumerable<ItemCreateDto> rows, CancellationToken ct);

        // StatusCode: 0=Updated, 1=NotFound, 2=Concurrency
        Task<IReadOnlyList<(int Id, byte[]? NewRowVersion, UpdateStatusCode StatusCode)>> UpdateManyTvpAsync(
            IEnumerable<ItemUpdateDto> rows, CancellationToken ct);

        void SetOriginalRowVersion(Item entity, string base64);
    }
}
