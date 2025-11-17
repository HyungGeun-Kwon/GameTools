using GameTools.Server.Domain.Common.Exceptions;

namespace GameTools.Server.Domain.Features.Items.Exceptions
{
    public sealed class ItemPriceTooSmallException(int minValue) 
        : DomainException($"Item price must be > {minValue}.");
}
