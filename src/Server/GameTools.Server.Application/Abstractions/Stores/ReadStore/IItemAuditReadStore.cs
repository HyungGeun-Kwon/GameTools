using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage;

namespace GameTools.Server.Application.Abstractions.Stores.ReadStore
{
    public interface IItemAuditReadStore
    {
        Task<PagedResult<ItemAuditReadModel>> GetItemAuditPageAsync(ItemAuditPageSearchCriteria criteria, CancellationToken ct);
    }
}
