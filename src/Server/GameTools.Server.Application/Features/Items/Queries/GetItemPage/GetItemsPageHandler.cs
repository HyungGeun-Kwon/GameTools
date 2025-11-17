using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Items.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Queries.GetItemPage
{
    public sealed class GetItemsPageHandler(IItemReadStore itemReadStore)
        : IRequestHandler<GetItemsPageQuery, PagedResult<ItemReadModel>>
    {
        public Task<PagedResult<ItemReadModel>> Handle(GetItemsPageQuery request, CancellationToken ct)
            => itemReadStore.GetPageAsync(request.Criteria, ct);
    }
}
