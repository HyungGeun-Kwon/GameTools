using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.Models.Audits;
using GameTools.Client.Application.UseCases.Audits.GetItemAuditsPage;

namespace GameTools.Client.Application.Ports
{
    public interface IAuditsGateway
    {
        Task<PagedOutput<ItemAudit>> GetPageAsync(GetItemAuditsPageInput input, CancellationToken ct);
    }
}
