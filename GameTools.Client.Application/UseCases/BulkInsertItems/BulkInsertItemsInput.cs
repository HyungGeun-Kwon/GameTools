using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTools.Contracts.Items.BulkInsertItems;

namespace GameTools.Client.Application.UseCases.BulkInsertItems
{
    public sealed record BulkInsertItemsInput(IReadOnlyList<BulkInsertItemInputRow> BulkInsertItems);
}
