using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Items.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Queries.GetItemPage
{
    public sealed record GetItemsPageQuery(ItemsPageSearchCriteria Criteria)
        : IRequest<PagedResult<ItemReadModel>>;
}
