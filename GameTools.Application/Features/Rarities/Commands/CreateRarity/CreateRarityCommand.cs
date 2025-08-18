using GameTools.Application.Features.Rarities.Dtos;
using MediatR;

namespace GameTools.Application.Features.Rarities.Commands.CreateRarity
{
    public record CreateRarityCommand(RarityCreateDto RarityCreateDto) : IRequest<RarityDto>
    {
        public string NormalizedColorCode => (RarityCreateDto.ColorCode ?? "").Trim().ToUpperInvariant();
    }
}