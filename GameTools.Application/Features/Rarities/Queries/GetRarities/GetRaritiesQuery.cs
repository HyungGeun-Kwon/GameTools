using GameTools.Application.Features.Rarities.Dtos;
using MediatR;

namespace GameTools.Application.Features.Rarities.Queries.GetRarities
{
    public sealed record GetRaritiesQuery : IRequest<List<RarityDto>>;
}
