using GameTools.Server.Application.Auditing.Queries.GetItemAuditPage;
using GameTools.Server.Application.Common.Paging;

namespace GameTools.Server.Application.Abstractions.Stores.ReadStore
{
    public interface IItemAuditReadStore : IReadStore<ItemAuditReadModel, Guid>
    {
        Task<PagedResult<ItemAuditReadModel>> GetItemAuditPageAsync(ItemAuditPageCriteria criteria, CancellationToken ct);
    }
}
