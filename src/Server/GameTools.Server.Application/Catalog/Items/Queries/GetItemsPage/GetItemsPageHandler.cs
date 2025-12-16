using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Catalog.Items.Models;
using GameTools.Server.Application.Common.Paging;
using MediatR;

namespace GameTools.Server.Application.Catalog.Items.Queries.GetItemsPage
{
    public sealed class GetItemsPageHandler(IItemReadStore itemReadStore)
        : IRequestHandler<GetItemsPageQuery, PagedResult<ItemReadModel>>
    {
        public Task<PagedResult<ItemReadModel>> Handle(GetItemsPageQuery query, CancellationToken ct)
            => itemReadStore.GetPageAsync(query.Criteria, ct);
    }
}
