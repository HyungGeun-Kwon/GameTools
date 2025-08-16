using GameTools.Application.Features.Rarities.Dtos;
using MediatR;

namespace GameTools.Application.Features.Rarities.Queries.GetRarityById
{
    public sealed record GetRarityByIdQuery(byte Id) : IRequest<RarityDto?>;
}
