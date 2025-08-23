using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Contracts.Items.CreateItem
{
    public sealed record CreateItemRequest(string Name, int Price, byte RarityId, string? Description = null);
}
