using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTools.Application.Common.Paging;

namespace GameTools.Application.Features.Items.Queries.GetItemPage
{
    public sealed record GetItemsPageQueryParams(Pagination Pagination, ItemFilter? Filter);
}
