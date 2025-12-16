using GameTools.Server.Domain.Common.Checkers;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Catalog.Rarities.Services
{
    public interface IRarityGradeUniquenessChecker : IUniquenessChecker<RarityGrade>;
}
