using GameTools.Server.Application.Catalog.Items.Models;
using GameTools.Server.Application.Common.Paging;
using MediatR;

namespace GameTools.Server.Application.Catalog.Items.Queries.GetItemsPage
{
    public sealed record GetItemsPageQuery(ItemsPageCriteria Criteria)
        : IRequest<PagedResult<ItemReadModel>>;
}
