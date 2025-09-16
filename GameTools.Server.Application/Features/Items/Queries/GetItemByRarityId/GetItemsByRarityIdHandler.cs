using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Features.Items.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Queries.GetItemById
{
    public sealed class GetItemsByRarityIdHandler(IItemReadStore itemReadStore) : IRequestHandler<GetItemsByRarityIdQuery, IReadOnlyList<ItemReadModel>>
    {
        public Task<IReadOnlyList<ItemReadModel>> Handle(GetItemsByRarityIdQuery request, CancellationToken ct)
            => itemReadStore.GetByRarityIdAsync(request.RarityId, ct);
    }
}
