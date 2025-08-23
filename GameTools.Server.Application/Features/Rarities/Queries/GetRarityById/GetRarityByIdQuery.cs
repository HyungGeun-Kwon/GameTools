using GameTools.Server.Application.Features.Rarities.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Queries.GetRarityById
{
    public sealed record GetRarityByIdQuery(byte Id) : IRequest<RarityReadModel?>;
}
