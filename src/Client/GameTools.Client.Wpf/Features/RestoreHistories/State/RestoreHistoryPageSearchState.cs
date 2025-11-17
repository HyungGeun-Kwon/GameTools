using GameTools.Client.Application.Models.Restores;
using GameTools.Client.Wpf.Shared.State;

namespace GameTools.Client.Wpf.Features.RestoreHistories.State
{
    public class RestoreHistoryPageSearchState : PageSearchState<RestoreHistory>, IRestoreHistoryPageSearchState
    {
        public DateTime? FromUtc { get; private set; }

        public DateTime? ToUtc { get; private set; }

        public string? Actor { get; private set; }

        public bool? DryOnly { get; private set; }

        public void ReplaceFilter(DateTime? fromUtc, DateTime? toUtc, string? actor, bool? dryOnly)
        {
            FromUtc = fromUtc;
            ToUtc = toUtc;
            Actor = actor;
            DryOnly = dryOnly;
        }
    }
}
