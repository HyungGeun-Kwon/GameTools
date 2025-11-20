using GameTools.Server.Application.Features.Rarities.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Queries.GetAllRarities
{
    public sealed record GetAllRaritiesQuery : IRequest<IReadOnlyList<RarityReadModel>>;
}
