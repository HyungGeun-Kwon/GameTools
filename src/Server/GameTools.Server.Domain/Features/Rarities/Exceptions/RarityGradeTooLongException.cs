using GameTools.Server.Domain.Common.Exceptions;

namespace GameTools.Server.Domain.Features.Rarities.Exceptions
{
    public sealed class RarityGradeTooLongException(int maxLength)
        : DomainException($"Rarity garde is too long (max {maxLength}).")
    {
    }
}
