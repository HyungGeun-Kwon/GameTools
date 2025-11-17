using GameTools.Server.Application.Common.Paging;
using MediatR;

namespace GameTools.Server.Application.Features.Restores.Queries.GetRestoreRunsPage
{
    public sealed record GetRestoreRunsPageQuery(RestoreRunPageSearchCriteria Criteria) : IRequest<PagedResult<RestoreRunReadModel>>;
}
