using GameTools.Server.Domain.Features.Rarities.Exceptions;
using GameTools.Server.Domain.Features.Rarities.Services;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Features.Rarities.Policies
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
