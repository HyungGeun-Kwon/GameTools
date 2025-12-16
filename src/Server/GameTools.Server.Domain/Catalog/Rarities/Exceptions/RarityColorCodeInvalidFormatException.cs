using GameTools.Server.Domain.Common.Exceptions;

namespace GameTools.Server.Domain.Catalog.Rarities.Exceptions
{
    public sealed class RarityColorCodeInvalidFormatException(string? value) 
        : DomainException($"Rarity color code '{value}' must be in the format '#RRGGBB' (uppercase).")
    {
    }
}
