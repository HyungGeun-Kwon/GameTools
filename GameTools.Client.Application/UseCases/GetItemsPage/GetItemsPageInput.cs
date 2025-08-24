using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTools.Client.Application.Common.Paging;

namespace GameTools.Client.Application.UseCases.GetItemsPage
{
    public sealed record GetItemsPageInput(Pagination Pagination, ItemSearchFilter? Filter);
}
