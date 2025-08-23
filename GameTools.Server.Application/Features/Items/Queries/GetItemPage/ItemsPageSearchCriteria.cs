using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTools.Server.Application.Common.Paging;

namespace GameTools.Server.Application.Features.Items.Queries.GetItemPage
{
    public sealed record ItemsPageSearchCriteria(Pagination Pagination, ItemsFilter? Filter);
}
