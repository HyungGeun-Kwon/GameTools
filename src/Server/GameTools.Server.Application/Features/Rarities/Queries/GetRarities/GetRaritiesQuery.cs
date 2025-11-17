using GameTools.Server.Application.Features.Rarities.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Queries.GetRarities
{
    public sealed record GetRaritiesQuery : IRequest<IReadOnlyList<RarityReadModel>>;
}
