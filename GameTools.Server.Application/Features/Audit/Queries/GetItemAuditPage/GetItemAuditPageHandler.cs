using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Common.Paging;
using MediatR;

namespace GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage
{
    public sealed class GetItemAuditPageHandler(IItemAuditReadStore store)
        : IRequestHandler<GetItemAuditPageQuery, PagedResult<ItemAuditReadModel>>
    {
        public Task<PagedResult<ItemAuditReadModel>> Handle(GetItemAuditPageQuery request, CancellationToken ct)
            => store.GetItemAuditPageAsync(request.Criteria, ct);
    }
}
