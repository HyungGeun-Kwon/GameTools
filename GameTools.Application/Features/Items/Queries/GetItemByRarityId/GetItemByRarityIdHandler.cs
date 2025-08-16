using GameTools.Application.Abstractions.ReadStore;
using GameTools.Application.Features.Items.Dtos;
using MediatR;

namespace GameTools.Application.Features.Items.Queries.GetItemById
{
    public sealed class GetItemByRarityIdHandler(IItemReadStore itemReadStore) : IRequestHandler<GetItemByRarityIdQuery, List<ItemDto>>
    {
        public async Task<List<ItemDto>> Handle(GetItemByRarityIdQuery request, CancellationToken ct)
            => await itemReadStore.GetByRarityIdAsync(request.rarityId, ct);
    }
}
