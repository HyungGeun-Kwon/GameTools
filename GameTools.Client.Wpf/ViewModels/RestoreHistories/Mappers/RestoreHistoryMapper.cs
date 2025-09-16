using GameTools.Client.Application.UseCases.Restores.GetRestoreHistoryPage;
using GameTools.Client.Wpf.Common.State;

namespace GameTools.Client.Wpf.ViewModels.RestoreHistories.Mappers
{
    public static class RestoreHistoryMapper
    {
        public static GetRestoreHistoriesPageInput ToGetRestoreHistoryPageInput(this IRestoreHistoryPageSearchState restoreHistoryPageSearchState)
            => new
            (
                new(restoreHistoryPageSearchState.PageNumber, restoreHistoryPageSearchState.PageSize),
                new(restoreHistoryPageSearchState.FromUtc, restoreHistoryPageSearchState.ToUtc, restoreHistoryPageSearchState.Actor, restoreHistoryPageSearchState.DryOnly)
            );

        public static GetRestoreHistoriesPageInput GetrestoreHistoryPageInputFromNewPage(this IRestoreHistoryPageSearchState restoreHistoryPageSearchState, int page)
            => new
            (
                new(page, restoreHistoryPageSearchState.PageSize),
                new(restoreHistoryPageSearchState.FromUtc, restoreHistoryPageSearchState.ToUtc, restoreHistoryPageSearchState.Actor, restoreHistoryPageSearchState.DryOnly)
            );
    }
}
