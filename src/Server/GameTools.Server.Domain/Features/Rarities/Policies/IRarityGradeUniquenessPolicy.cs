using GameTools.Server.Domain.Common.Policies;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Features.Rarities.Policies
{
    public interface IRarityGradeUniquenessPolicy : IUniquenessPolicy<RarityGrade>;
}
