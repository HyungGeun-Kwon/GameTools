using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Client.Application.UseCases.BulkInsertItems
{
    public sealed record BulkInsertItemOutputRow(int Id, string RowVersionBase64);
}
