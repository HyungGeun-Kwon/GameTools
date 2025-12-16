using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Common.Paging;
using MediatR;

namespace GameTools.Server.Application.Auditing.Queries.GetItemAuditPage
{
    public sealed class GetItemAuditPageHandler(IItemAuditReadStore store)
        : IRequestHandler<GetItemAuditPageQuery, PagedResult<ItemAuditReadModel>>
    {
        public Task<PagedResult<ItemAuditReadModel>> Handle(GetItemAuditPageQuery query, CancellationToken ct)
            => store.GetItemAuditPageAsync(query.Criteria, ct);
    }
}
