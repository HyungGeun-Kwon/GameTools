using GameTools.Server.Application.Features.Rarities.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Commands.UpdateRarity
{
    public sealed record UpdateRarityCommand(UpdateRaritySpec Spec) : IRequest<UpdateRarityResult>;
}
