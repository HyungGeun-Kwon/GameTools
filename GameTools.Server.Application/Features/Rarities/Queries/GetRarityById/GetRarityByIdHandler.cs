using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Features.Rarities.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Queries.GetRarityById
{
    public sealed class GetRarityByIdHandler(IRarityReadStore rarityReadStore) : IRequestHandler<GetRarityByIdQuery, RarityReadModel?>
    {
        public async Task<RarityReadModel?> Handle(GetRarityByIdQuery request, CancellationToken ct)
            => await rarityReadStore.GetByIdAsync(request.Id, ct);
    }
}
