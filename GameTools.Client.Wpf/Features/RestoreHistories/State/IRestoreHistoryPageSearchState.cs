using GameTools.Client.Application.Models.Restores;
using GameTools.Client.Wpf.Shared.State;

namespace GameTools.Client.Wpf.Features.RestoreHistories.State
{
    public interface IRestoreHistoryPageSearchState : IPageSearchState<RestoreHistory>
    {
        DateTime? FromUtc { get; }
        DateTime? ToUtc { get; }
        string? Actor { get; }
        bool? DryOnly { get; }
        void ReplaceFilter(DateTime? fromUtc, DateTime? toUtc, string? actor, bool? dryOnly);
    }
}
