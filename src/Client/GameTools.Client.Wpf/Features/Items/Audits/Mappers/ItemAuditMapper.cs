using GameTools.Client.Application.UseCases.Audits.GetItemAuditsPage;
using GameTools.Client.Wpf.Features.Items.Audits.State;

namespace GameTools.Client.Wpf.Features.Items.Audits.Mappers
{
    public static class ItemAuditMapper
    {
        public static GetItemAuditsPageInput ToGetItemAuditPageInput(this IItemAuditPageSearchState itemAuditPageSearchState)
             => new
             (
                 new(itemAuditPageSearchState.PageNumber, itemAuditPageSearchState.PageSize),
                 new
                 (
                     itemAuditPageSearchState.ItemId, 
                     itemAuditPageSearchState.Action, 
                     itemAuditPageSearchState.FromUtc, 
                     itemAuditPageSearchState.ToUtc
                 )
             );

        public static GetItemAuditsPageInput GetItemAuditPageInputFromNewPage(this IItemAuditPageSearchState itemAuditPageSearchState, int page)
            => new
            (
                new(page, itemAuditPageSearchState.PageSize),
                 new
                 (
                     itemAuditPageSearchState.ItemId,
                     itemAuditPageSearchState.Action,
                     itemAuditPageSearchState.FromUtc,
                     itemAuditPageSearchState.ToUtc
                 )
            );
    }
}
