using GameTools.Server.Domain.Catalog.Items.Exceptions;
using GameTools.Server.Domain.Catalog.Items.Services;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;

namespace GameTools.Server.Domain.Catalog.Items.Policies
{
    public sealed class ItemNameUniquenessPolicy(IItemNameUniquenessChecker checker) : IItemNameUniquenessPolicy
    {
        public async Task EnsureUniqueAsync(ItemName name, CancellationToken ct)
        {
            if (await checker.ExistsAsync(name, ct))
                throw new ItemNameNotUniqueException(name);
        }
    }
}
