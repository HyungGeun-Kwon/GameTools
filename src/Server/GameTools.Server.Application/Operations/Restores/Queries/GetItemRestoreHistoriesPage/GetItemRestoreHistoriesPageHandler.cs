using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Common.Paging;
using MediatR;

namespace GameTools.Server.Application.Operations.Restores.Queries.GetItemRestoreHistoriesPage
{
    public sealed class GetItemRestoreHistoriesPageHandler(IItemRestoreHistoryReadStore store)
        : IRequestHandler<GetItemRestoreHistoriesPageQuery, PagedResult<ItemRestoreHistoriesReadModel>>
    {
        public Task<PagedResult<ItemRestoreHistoriesReadModel>> Handle(GetItemRestoreHistoriesPageQuery request, CancellationToken ct)
            => store.GetItemRestoreHistoriesPageAsync(request.Criteria, ct);
    }
}
