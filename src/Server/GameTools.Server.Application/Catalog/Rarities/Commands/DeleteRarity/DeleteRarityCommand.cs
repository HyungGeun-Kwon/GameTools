using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Specs;
using MediatR;

namespace GameTools.Server.Application.Catalog.Rarities.Commands.DeleteRarity
{
    public sealed record DeleteRarityCommand(DeleteRaritySpec Spec) : IRequest<DeleteRarityResult>;
}
