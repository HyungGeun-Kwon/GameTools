using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Catalog.Rarities.Commands.UpdateRarity
{
    public sealed record UpdateRarityCommand(UpdateRaritySpec Spec) : IRequest<UpdateRarityResult>;
}
