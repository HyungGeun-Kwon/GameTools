using GameTools.Application.Features.Rarities.Dtos;
using MediatR;

namespace GameTools.Application.Features.Rarities.Commands.DeleteRarity
{
    public sealed record DeleteRarityCommand(RarityDeleteDto RarityDeleteDto) : IRequest;
}
