using GameTools.Server.Domain.Common.Policies;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;

namespace GameTools.Server.Domain.Catalog.Items.Policies
{
    public interface IItemNameUniquenessPolicy : IUniquenessPolicy<ItemName>;
}
