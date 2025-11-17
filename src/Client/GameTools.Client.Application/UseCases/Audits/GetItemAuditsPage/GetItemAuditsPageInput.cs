using GameTools.Client.Application.Common.Paging;

namespace GameTools.Client.Application.UseCases.Audits.GetItemAuditsPage
{
    public sealed record GetItemAuditsPageInput(Pagination Pagination, ItemAuditSearchFilter? Filter);
}
