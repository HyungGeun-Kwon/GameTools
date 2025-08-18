using GameTools.Application.Features.Rarities.Dtos;
using MediatR;

namespace GameTools.Application.Features.Rarities.Commands.UpdateRarity
{
    public sealed record UpdateRarityCommand(RarityUpdateDto RarityUpdateDto) : IRequest<RarityDto>
    {
        public string NormalizedColorCode => (RarityUpdateDto.ColorCode ?? "").Trim().ToUpperInvariant();
    }
}
