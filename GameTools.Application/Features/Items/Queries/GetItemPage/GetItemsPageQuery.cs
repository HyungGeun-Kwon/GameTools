using GameTools.Application.Common.Paging;
using GameTools.Application.Features.Items.Dtos;
using MediatR;

namespace GameTools.Application.Features.Items.Queries.GetItemPage
{
    public sealed record GetItemsPageQuery(
        int PageNumber,
        int PageSize,
        string? Search, // Name 부분 일치(contains)
        byte? RarityId
    ) : IRequest<PagedResult<ItemDto>>;
}
