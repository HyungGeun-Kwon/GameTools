using GameTools.Application.Features.Rarities.Dtos;
using MediatR;

namespace GameTools.Application.Features.Rarities.Commands.UpdateRarity
{
    public sealed record UpdateRarityCommand(byte Id, string Grade, string ColorCode) : IRequest<RarityDto>
    {
        public string NormalizedColorCode => (ColorCode ?? "").Trim().ToUpperInvariant();
    }
}
