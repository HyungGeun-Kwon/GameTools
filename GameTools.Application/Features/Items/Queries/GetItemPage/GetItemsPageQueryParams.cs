using GameTools.Application.Common.Paging;

namespace GameTools.Application.Features.Items.Queries.GetItemPage
{
    public sealed record GetItemsPageQueryParams(Pagination Pagination, ItemFilter? Filter);
}
