using System.Net.Http.Json;
using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.Models.Audits;
using GameTools.Client.Application.Ports;
using GameTools.Client.Application.UseCases.Audits.GetItemAuditsPage;
using GameTools.Client.Infrastructure.Mapper;
using GameTools.Contracts.Audits.Common;
using GameTools.Contracts.Common;
using GameTools.Contracts.Items.GetItemAuditPage;

namespace GameTools.Client.Infrastructure.Http
{
    public sealed class AuditsGateway(HttpClient http) : IAuditsGateway
    {
        public async Task<PagedOutput<ItemAudit>> GetPageAsync(GetItemAuditsPageInput input, CancellationToken ct)
        {
            ItemAuditsPageRequest req = input.ToContract();
            using var res = await http.PostAsJsonAsync("/api/audits/items-page-search", req, ct);
            res.EnsureSuccessStatusCode();

            var page = await res.Content.ReadFromJsonAsync<PagedResponse<ItemAuditResponse>>(cancellationToken: ct)
                ?? new PagedResponse<ItemAuditResponse>([], 0, req.PageNumber, req.PageSize);

            return page.ToClient();
        }
    }
}
