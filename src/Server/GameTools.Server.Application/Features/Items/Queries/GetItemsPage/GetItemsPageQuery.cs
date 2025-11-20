using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Items.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Queries.GetItemsPage
{
    public sealed record GetItemsPageQuery(ItemsPageCriteria Criteria)
        : IRequest<PagedResult<ItemReadModel>>;
}
