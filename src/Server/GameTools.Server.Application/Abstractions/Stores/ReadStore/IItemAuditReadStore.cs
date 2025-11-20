using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage;

namespace GameTools.Server.Application.Abstractions.Stores.ReadStore
{
    public interface IItemAuditReadStore : IReadStore<ItemAuditReadModel, Guid>
    {
        Task<PagedResult<ItemAuditReadModel>> GetItemAuditPageAsync(ItemAuditPageCriteria criteria, CancellationToken ct);
    }
}
