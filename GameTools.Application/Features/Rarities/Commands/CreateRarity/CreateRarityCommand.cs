using GameTools.Application.Features.Rarities.Dtos;
using MediatR;

namespace GameTools.Application.Features.Rarities.Commands.CreateRarity
{
    public record CreateRarityCommand(string Grade, string ColorCode) : IRequest<RarityDto>
    {
        public string NormalizedColorCode => (ColorCode ?? "").Trim().ToUpperInvariant();
    }
}