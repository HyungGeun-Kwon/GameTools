using GameTools.Server.Application.Catalog.Rarities.Models;
using MediatR;

namespace GameTools.Server.Application.Catalog.Rarities.Queries.GetAllRarities
{
    public sealed record GetAllRaritiesQuery : IRequest<IReadOnlyList<RarityReadModel>>;
}
