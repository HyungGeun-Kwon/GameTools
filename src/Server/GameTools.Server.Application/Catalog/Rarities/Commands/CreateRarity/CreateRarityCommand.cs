using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Catalog.Rarities.Commands.CreateRarity
{
    public sealed record CreateRarityCommand(CreateRaritySpec Spec) : IRequest<CreateRarityResult>;
}