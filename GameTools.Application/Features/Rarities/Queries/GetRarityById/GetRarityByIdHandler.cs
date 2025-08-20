using GameTools.Application.Abstractions.Stores.ReadStore;
using GameTools.Application.Features.Rarities.Dtos;
using MediatR;

namespace GameTools.Application.Features.Rarities.Queries.GetRarityById
{
    public sealed class GetRarityByIdHandler(IRarityReadStore rarityReadStore) : IRequestHandler<GetRarityByIdQuery, RarityDto?>
    {
        public async Task<RarityDto?> Handle(GetRarityByIdQuery request, CancellationToken ct)
            => await rarityReadStore.GetByIdAsync(request.Id, ct);
    }
}
