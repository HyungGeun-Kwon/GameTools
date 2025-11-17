using GameTools.Server.Domain.Common.Exceptions;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Features.Rarities.Exceptions
{
    public sealed class RarityColorCodeNotUniqueException(RarityColorCode code)
        : DomainException($"'{code.Value}' already exists. Rarity colorCode must be unique.")
    {
    }
}
