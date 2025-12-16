using GameTools.Server.Domain.Catalog.Rarities.Exceptions;
using GameTools.Server.Domain.Catalog.Rarities.Services;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Catalog.Rarities.Policies
{
    public sealed class RarityGradeUniquenessPolicy(IRarityGradeUniquenessChecker checker) : IRarityGradeUniquenessPolicy
    {
        public async Task EnsureUniqueAsync(RarityGrade grade, CancellationToken ct)
        {
            if (await checker.ExistsAsync(grade, ct))
                throw new RarityGradeNotUniqueException(grade);
        }
    }
}
