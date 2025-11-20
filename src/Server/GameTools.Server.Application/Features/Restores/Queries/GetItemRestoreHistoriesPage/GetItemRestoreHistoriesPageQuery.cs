using GameTools.Server.Application.Common.Paging;
using MediatR;

namespace GameTools.Server.Application.Features.Restores.Queries.GetItemRestoreHistoriesPage
{
    public sealed record GetItemRestoreHistoriesPageQuery(ItemRestoreHistoriesPageCriteria Criteria) : IRequest<PagedResult<ItemRestoreHistoriesReadModel>>;
}
