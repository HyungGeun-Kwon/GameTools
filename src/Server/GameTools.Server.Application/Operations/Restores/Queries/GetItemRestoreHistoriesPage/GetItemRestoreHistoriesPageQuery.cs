using GameTools.Server.Application.Common.Paging;
using MediatR;

namespace GameTools.Server.Application.Operations.Restores.Queries.GetItemRestoreHistoriesPage
{
    public sealed record GetItemRestoreHistoriesPageQuery(ItemRestoreHistoriesPageCriteria Criteria) : IRequest<PagedResult<ItemRestoreHistoriesReadModel>>;
}
