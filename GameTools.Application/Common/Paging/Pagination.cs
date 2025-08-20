using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Application.Common.Paging
{
    public sealed record Pagination(int PageNumber = 1, int PageSize = 20);
}
