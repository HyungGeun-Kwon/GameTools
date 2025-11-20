using GameTools.Server.Application.Features.Rarities.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Commands.CreateRarity
{
    public sealed record CreateRarityCommand(CreateRaritySpec Spec) : IRequest<CreateRarityResult>;
}