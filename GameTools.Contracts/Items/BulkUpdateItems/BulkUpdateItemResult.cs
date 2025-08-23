using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Contracts.Items.BulkUpdateItems
{
    public sealed record BulkUpdateItemResult(int Id, string Status, string? NewRowVersionBase64);
}
