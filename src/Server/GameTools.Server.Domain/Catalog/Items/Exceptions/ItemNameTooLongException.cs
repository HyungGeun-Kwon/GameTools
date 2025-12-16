using GameTools.Server.Domain.Common.Exceptions;

namespace GameTools.Server.Domain.Catalog.Items.Exceptions
{
    public sealed class ItemNameTooLongException(int maxLength) 
        : DomainException($"Item name is too long (max {maxLength}).");
}
