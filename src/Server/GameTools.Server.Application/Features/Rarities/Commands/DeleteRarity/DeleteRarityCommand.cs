using GameTools.Server.Application.Common.Results;
using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Commands.DeleteRarity
{
    public sealed record DeleteRarityCommand(DeleteRarityPayload Payload) : IRequest<WriteStatusCode>;
}
