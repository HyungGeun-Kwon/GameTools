using GameTools.Application.Abstractions.Stores.ReadStore;
using GameTools.Application.Features.Items.Dtos;
using MediatR;

namespace GameTools.Application.Features.Items.Queries.GetItemById
{
    public sealed class GetItemByRarityIdHandler(IItemReadStore itemReadStore) : IRequestHandler<GetItemByRarityIdQuery, IReadOnlyList<ItemDto>>
    {
        public async Task<IReadOnlyList<ItemDto>> Handle(GetItemByRarityIdQuery request, CancellationToken ct)
            => await itemReadStore.GetByRarityIdAsync(request.RarityId, ct);
    }
}
