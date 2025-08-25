using GameTools.Server.Application.Common.Results;
using GameTools.Server.Application.Features.Rarities.Models;

namespace GameTools.Server.Application.Features.Rarities.Commands.UpdateRarity
{
    public sealed record UpdateRarityResult(WriteStatusCode WriteStatusCode, RarityReadModel? RarityReadModel);
}
