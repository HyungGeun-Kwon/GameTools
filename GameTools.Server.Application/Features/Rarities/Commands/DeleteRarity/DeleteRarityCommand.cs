using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Commands.DeleteRarity
{
    public sealed record DeleteRarityCommand(DeleteRarityPayload Payload) : IRequest;
}
