using GameTools.Server.Application.Abstractions.Exceptions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Features.Rarities.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Queries.GetRarityById
{
    public sealed class GetRarityByIdHandler(IRarityReadStore rarityReadStore) 
        : IRequestHandler<GetRarityByIdQuery, RarityReadModel>
    {
        public async Task<RarityReadModel> Handle(GetRarityByIdQuery query, CancellationToken ct)
        {
            var rarityReadModel = await rarityReadStore.GetByIdAsync(query.Id, ct)
                ?? throw new NotFoundException($"Rarity '{query.Id}' not found.");

            return rarityReadModel;
        }
    }
}
