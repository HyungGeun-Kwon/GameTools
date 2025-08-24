using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Client.Application.UseCases.DeleteItem
{
    public sealed record DeleteItemInput(int Id, string RowVersionBase64);
}
