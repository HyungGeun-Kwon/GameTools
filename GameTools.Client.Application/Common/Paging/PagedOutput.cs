using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Client.Application.Common.Paging
{
    public sealed record PagedOutput<T>(IReadOnlyList<T> Items, int TotalCount, int PageNumber, int PageSize);
}
