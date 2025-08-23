using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTools.Contracts.Items.Common;

namespace GameTools.Contracts.Items.GetItemsByRarity
{
    public sealed record ItemsByRarityResponse(IReadOnlyList<ItemResponse> Items);
}
