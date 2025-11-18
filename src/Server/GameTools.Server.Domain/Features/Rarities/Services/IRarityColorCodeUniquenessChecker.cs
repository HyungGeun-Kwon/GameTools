using GameTools.Server.Domain.Common.Checkers;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Features.Rarities.Services
{
    public interface IRarityColorCodeUniquenessChecker : IUniquenessChecker<RarityColorCode>;
}
