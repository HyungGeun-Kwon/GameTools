using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Features.Items.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Queries.GetItemById
{
    public sealed class GetItemsByRarityIdHandler(IItemReadStore itemReadStore) : IRequestHandler<GetItemsByRarityIdQuery, IReadOnlyList<ItemReadModel>>
    {
        public async Task<IReadOnlyList<ItemReadModel>> Handle(GetItemsByRarityIdQuery request, CancellationToken ct)
            => await itemReadStore.GetByRarityIdAsync(request.RarityId, ct);
    }
}
