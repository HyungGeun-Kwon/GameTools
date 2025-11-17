using GameTools.Server.Domain.Common.Exceptions;

namespace GameTools.Server.Domain.Features.Items.Exceptions
{
    public sealed class ItemDescriptionTooLongException(int maxLength)
        : DomainException($"Item description is too long (max {maxLength}).");
}
