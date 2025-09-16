using GameTools.Client.Application.Common.Paging;

namespace GameTools.Client.Application.UseCases.Restores.GetRestoreHistoryPage
{
    public sealed record GetRestoreHistoriesPageInput(Pagination Pagination, RestoreHistorySearchFilter? Filter);
}
