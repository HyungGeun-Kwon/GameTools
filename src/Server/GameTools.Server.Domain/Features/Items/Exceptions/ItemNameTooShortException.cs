using GameTools.Server.Domain.Common.Exceptions;

namespace GameTools.Server.Domain.Features.Items.Exceptions
{
    public sealed class ItemNameTooShortException(int minLength) 
        : DomainException($"Item name is too short (min {minLength}).");
}
