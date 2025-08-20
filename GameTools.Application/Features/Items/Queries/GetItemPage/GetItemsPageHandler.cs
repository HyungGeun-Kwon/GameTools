using GameTools.Application.Abstractions.Stores.ReadStore;
using GameTools.Application.Common.Paging;
using GameTools.Application.Features.Items.Dtos;
using MediatR;

namespace GameTools.Application.Features.Items.Queries.GetItemPage
{
    public sealed class GetItemsPageHandler(IItemReadStore itemReadStore)
        : IRequestHandler<GetItemsPageQuery, PagedResult<ItemDto>>
    {
        public async Task<PagedResult<ItemDto>> Handle(GetItemsPageQuery request, CancellationToken ct)
            => await itemReadStore.GetPageAsync(request.PageNumber, request.PageSize, request.Search, request.RarityId, ct);
    }
}
