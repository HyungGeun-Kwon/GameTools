using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Contracts.Items.InsertItemsTvp
{
    public sealed record BulkInsertItemsRequest(IReadOnlyList<BulkInsertItemRow> BulkInsertItems);
}
