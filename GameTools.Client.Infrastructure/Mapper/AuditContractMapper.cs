using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.Models.Audits;
using GameTools.Client.Application.UseCases.Audits.GetItemAuditsPage;
using GameTools.Contracts.Audits.Common;
using GameTools.Contracts.Common;
using GameTools.Contracts.Items.GetItemAuditPage;

namespace GameTools.Client.Infrastructure.Mapper
{
    public static class AuditContractMapper
    {
        public static ItemAuditsPageRequest ToContract(this GetItemAuditsPageInput input)
            => new(
                input.Pagination.PageNumber,
                input.Pagination.PageSize,
                input.Filter?.ItemId,
                input.Filter?.Action,
                input.Filter?.FromUtc,
                input.Filter?.ToUtc
            );

        public static ItemAudit ToClient(this ItemAuditResponse res)
            => new(res.AuditId, res.Action, res.BeforeJson, res.AfterJson, res.ChangedAtUtc, res.ChangedBy);

        public static PagedOutput<ItemAudit> ToClient(this PagedResponse<ItemAuditResponse> p)
            => new(p.Items.Select(ToClient).ToList(), p.TotalCount, p.PageNumber, p.PageSize);
    }
}
