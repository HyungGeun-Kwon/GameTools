using GameTools.Server.Application.Common.Paging;
using MediatR;

namespace GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage
{
    public sealed record GetItemAuditPageQuery(ItemAuditPageSearchCriteria Criteria) 
        : IRequest<PagedResult<ItemAuditReadModel>>;
}
