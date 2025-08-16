using GameTools.Application.Abstractions.ReadStore;
using GameTools.Application.Features.Rarities.Dtos;
using MediatR;

namespace GameTools.Application.Features.Rarities.Queries.GetRarities
{
    public sealed class GetRaritiesHandler(IRarityReadStore rarityReadStore)
        : IRequestHandler<GetRaritiesQuery, List<RarityDto>>
    {
        public async Task<List<RarityDto>> Handle(GetRaritiesQuery request, CancellationToken ct)
            => await rarityReadStore.GetAllAsync(ct);
    }
}
