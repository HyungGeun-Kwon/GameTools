using GameTools.Server.Domain.Common.Exceptions;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Features.Rarities.Exceptions
{
    public sealed class RarityGradeNotUniqueException(RarityGrade grade)
        : DomainException($"'{grade.Value}' already exists. Rarity garde must be unique.")
    {
    }
}
