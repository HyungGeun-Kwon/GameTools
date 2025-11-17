using GameTools.Server.Domain.Features.Items.ValueObjects;

namespace GameTools.Server.Domain.Features.Items.Policies
{
    public interface IItemNameUniquenessChecker
    {
        Task<bool> ExistsAsync(ItemName name, CancellationToken ct);
    }
}
