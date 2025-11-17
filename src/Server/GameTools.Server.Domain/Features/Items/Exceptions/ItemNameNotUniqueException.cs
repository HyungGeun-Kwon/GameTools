using GameTools.Server.Domain.Common.Exceptions;
using GameTools.Server.Domain.Features.Items.ValueObjects;

namespace GameTools.Server.Domain.Features.Items.Exceptions
{
    public sealed class ItemNameNotUniqueException(ItemName name) 
        : DomainException($"'{name.Value}' already exists. Item name must be unique.");
}
