using GameTools.Client.Application.Models.Audits;

namespace GameTools.Client.Wpf.Common.State
{
    public sealed class ItemAuditPageSearchState : PageSearchState<ItemAudit>, IItemAuditPageSearchState
    {
        public int? ItemId { get; private set; }
        public string? Action { get; private set; }
        public DateTime? FromUtc { get; private set; }
        public DateTime? ToUtc { get; private set; }

        public void ReplaceFilter(int? itemId, string? action, DateTime? fromUtc, DateTime? toUtc)
        {
            ItemId = itemId;
            Action = action;
            FromUtc = fromUtc;
            ToUtc = toUtc;
        }
    }
}
