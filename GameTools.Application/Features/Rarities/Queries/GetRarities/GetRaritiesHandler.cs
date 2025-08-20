using GameTools.Application.Abstractions.Stores.ReadStore;
using GameTools.Application.Features.Rarities.Dtos;
using MediatR;

namespace GameTools.Application.Features.Rarities.Queries.GetRarities
{
    public sealed class GetRaritiesHandler(IRarityReadStore rarityReadStore)
        : IRequestHandler<GetRaritiesQuery, IReadOnlyList<RarityDto>>
    {
        public async Task<IReadOnlyList<RarityDto>> Handle(GetRaritiesQuery request, CancellationToken ct)
            => await rarityReadStore.GetAllAsync(ct);
    }
}
