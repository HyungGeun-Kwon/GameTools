using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Common.Paging;
using MediatR;

namespace GameTools.Server.Application.Features.Restores.Queries.GetRestoreRunsPage
{
    public sealed class GetRestoreRunsPageHandler(IRestoreRunReadStore store)
        : IRequestHandler<GetRestoreRunsPageQuery, PagedResult<RestoreRunReadModel>>
    {
        public Task<PagedResult<RestoreRunReadModel>> Handle(GetRestoreRunsPageQuery request, CancellationToken ct)
            => store.GetRestoreRunsPageAsync(request.Criteria, ct);
    }
}
