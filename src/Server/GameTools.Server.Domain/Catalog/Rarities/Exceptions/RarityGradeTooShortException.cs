using GameTools.Server.Domain.Common.Exceptions;

namespace GameTools.Server.Domain.Catalog.Rarities.Exceptions
{
    public sealed class RarityGradeTooShortException(int minLength)
        : DomainException($"Rarity grade is too short (min {minLength}).")
    {
    }
}
