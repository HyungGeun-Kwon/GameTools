using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Features.Rarities.Policies.Grade
{
    public interface IRarityGradeUniquenessChecker
    {
        Task<bool> ExistsAsync(RarityGrade grade, CancellationToken ct);
    }
}
