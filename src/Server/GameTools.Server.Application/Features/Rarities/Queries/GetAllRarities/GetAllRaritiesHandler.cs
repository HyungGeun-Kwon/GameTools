using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Features.Rarities.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Queries.GetAllRarities
{
    public sealed class GetAllRaritiesHandler(IRarityReadStore rarityReadStore)
        : IRequestHandler<GetAllRaritiesQuery, IReadOnlyList<RarityReadModel>>
    {
        public Task<IReadOnlyList<RarityReadModel>> Handle(GetAllRaritiesQuery query, CancellationToken ct)
            => rarityReadStore.GetAllAsync(ct);
    }
}
