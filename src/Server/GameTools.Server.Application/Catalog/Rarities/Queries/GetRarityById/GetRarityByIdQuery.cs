using GameTools.Server.Application.Catalog.Rarities.Models;
using MediatR;

namespace GameTools.Server.Application.Catalog.Rarities.Queries.GetRarityById
{
    public sealed record GetRarityByIdQuery(Guid Id) : IRequest<RarityReadModel>;
}
