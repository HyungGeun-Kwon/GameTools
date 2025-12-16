using GameTools.Server.Domain.Common.Policies;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Catalog.Rarities.Policies
{
    public interface IRarityColorCodeUniquenessPolicy : IUniquenessPolicy<RarityColorCode>;
}
