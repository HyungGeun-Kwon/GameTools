using GameTools.Server.Domain.Common.Policies;
using GameTools.Server.Domain.Features.Items.ValueObjects;

namespace GameTools.Server.Domain.Features.Items.Policies
{
    public interface IItemNameUniquenessPolicy : IUniquenessPolicy<ItemName>;
}
