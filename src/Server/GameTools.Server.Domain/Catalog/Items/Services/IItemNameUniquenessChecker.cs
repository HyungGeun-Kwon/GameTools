using GameTools.Server.Domain.Common.Checkers;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;

namespace GameTools.Server.Domain.Catalog.Items.Services
{
    public interface IItemNameUniquenessChecker : IUniquenessChecker<ItemName>;
}
