using GameTools.Contracts.Audits.Common;
using GameTools.Contracts.Common;
using GameTools.Contracts.Items.GetItemAuditPage;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage;

namespace GameTools.Server.Api.Mapper
{
    internal static class AuditApiMapper
    {
        internal static ItemAuditPageSearchCriteria ToCriteria(this ItemAuditsPageRequest req)
            => new(new Pagination(req.PageNumber, req.PageSize), new ItemAuditFilter(req.ItemId, req.Action, req.FromUtc, req.ToUtc));

        internal static ItemAuditResponse ToResponse(this ItemAuditReadModel readModel)
            => new
            (
                readModel.AuditId, 
                readModel.ItemId, 
                readModel.Action, 
                readModel.BeforeJson, 
                readModel.AfterJson, 
                readModel.ChangedAtUtc, 
                readModel.ChangedBy);

        internal static PagedResponse<ItemAuditResponse> ToResponse(this PagedResult<ItemAuditReadModel> pagedResult)
            => new
            (
                pagedResult.Items.Select(ToResponse).ToList(),
                pagedResult.TotalCount,
                pagedResult.PageNumber,
                pagedResult.PageSize
            );

    }
}
