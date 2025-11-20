using GameTools.Server.Application.Features.Rarities.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Commands.DeleteRarity
{
    public sealed record DeleteRarityCommand(DeleteRaritySpec Spec) : IRequest<DeleteRarityResult>;
}
