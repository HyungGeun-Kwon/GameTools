using GameTools.Server.Application.Features.Rarities.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Commands.UpdateRarity
{
    public sealed record UpdateRarityCommand(UpdateRarityPayload Payload) : IRequest<UpdateRarityResult>
    {
        public string NormalizedColorCode => (Payload.ColorCode ?? "").Trim().ToUpperInvariant();
    }
}
