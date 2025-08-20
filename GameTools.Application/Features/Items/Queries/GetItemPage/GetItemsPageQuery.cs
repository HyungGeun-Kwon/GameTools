using GameTools.Application.Common.Paging;
using GameTools.Application.Features.Items.Dtos;
using MediatR;

namespace GameTools.Application.Features.Items.Queries.GetItemPage
{
    public sealed record GetItemsPageQuery(GetItemsPageQueryParams GetItemsPageQueryParams)
        : IRequest<PagedResult<ItemDto>>;
}
