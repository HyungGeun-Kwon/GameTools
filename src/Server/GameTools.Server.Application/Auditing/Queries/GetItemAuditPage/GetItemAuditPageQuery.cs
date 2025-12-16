using GameTools.Server.Application.Common.Paging;
using MediatR;

namespace GameTools.Server.Application.Auditing.Queries.GetItemAuditPage
{
    public sealed record GetItemAuditPageQuery(ItemAuditPageCriteria Criteria) 
        : IRequest<PagedResult<ItemAuditReadModel>>;
}
