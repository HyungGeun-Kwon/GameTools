using GameTools.Contracts.Items.Common;

namespace GameTools.Contracts.Items.GetItemsByRarity
{
    public sealed record ItemsByRarityResponse(IReadOnlyList<ItemResponse> Items);
}
