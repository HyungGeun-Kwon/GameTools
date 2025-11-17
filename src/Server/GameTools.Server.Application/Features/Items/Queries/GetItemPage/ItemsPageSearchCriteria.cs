using GameTools.Server.Application.Common.Paging;

namespace GameTools.Server.Application.Features.Items.Queries.GetItemPage
{
    public sealed record ItemsPageSearchCriteria(Pagination Pagination, ItemsFilter? Filter);
}
