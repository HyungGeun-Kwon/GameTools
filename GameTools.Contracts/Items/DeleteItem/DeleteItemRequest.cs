using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Contracts.Items.DeleteItem
{
    public sealed record DeleteItemRequest(int Id, string RowVersionBase64);
}
