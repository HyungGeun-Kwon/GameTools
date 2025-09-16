using GameTools.Client.Application.Models.Audits;

namespace GameTools.Client.Wpf.Common.State
{
    public interface IItemAuditPageSearchState : IPageSearchState<ItemAudit>
    {
        int? ItemId { get; }
        string? Action { get; }
        DateTime? FromUtc { get; }
        DateTime? ToUtc { get; }

        void ReplaceFilter(int? itemId, string? action, DateTime? fromUtc, DateTime? toUtc);
    }
}
