using GameTools.Server.Domain.Features.Items.Exceptions;
using GameTools.Server.Domain.Features.Items.ValueObjects;

namespace GameTools.Server.Domain.Features.Items.Policies
{
    public sealed class ItemNameUniquenessPolicy(IItemNameUniquenessChecker checker)
    {
        public async Task EnsureUniqueAsync(ItemName name, CancellationToken ct)
        {
            if (await checker.ExistsAsync(name, ct))
                throw new ItemNameNotUniqueException(name);
        }
    }
}
