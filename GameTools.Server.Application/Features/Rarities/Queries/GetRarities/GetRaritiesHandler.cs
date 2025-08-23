using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Features.Rarities.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Queries.GetRarities
{
    public sealed class GetRaritiesHandler(IRarityReadStore rarityReadStore)
        : IRequestHandler<GetRaritiesQuery, IReadOnlyList<RarityReadModel>>
    {
        public async Task<IReadOnlyList<RarityReadModel>> Handle(GetRaritiesQuery request, CancellationToken ct)
            => await rarityReadStore.GetAllAsync(ct);
    }
}
