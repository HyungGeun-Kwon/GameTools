using GameTools.Server.Application.Features.Rarities.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Commands.CreateRarity
{
    public sealed record CreateRarityCommand(CreateRarityPayload Payload) : IRequest<RarityReadModel>
    {
        public string NormalizedColorCode => (Payload.ColorCode ?? "").Trim().ToUpperInvariant();
    }
}