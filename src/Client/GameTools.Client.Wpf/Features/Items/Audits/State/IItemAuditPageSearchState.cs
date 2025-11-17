using GameTools.Client.Application.Models.Audits;
using GameTools.Client.Wpf.Shared.State;

namespace GameTools.Client.Wpf.Features.Items.Audits.State
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
