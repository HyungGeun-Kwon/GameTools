using System.Net.Http.Json;
using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.Models.Restores;
using GameTools.Client.Application.Ports;
using GameTools.Client.Application.UseCases.Restores.GetRestoresPage;
using GameTools.Client.Infrastructure.Mapper;
using GameTools.Contracts.Common;
using GameTools.Contracts.Restores.Common;
using GameTools.Contracts.Restores.GetRestoresPage;

namespace GameTools.Client.Infrastructure.Http
{
    public sealed class RestoresGateway(HttpClient http) : IRestoreGateway
    {
        public async Task<PagedOutput<Restore>> GetPageAsync(GetRestoresPageInput input, CancellationToken ct)
        {
            RestoreRunPageRequest req = input.ToContract();
            using var res = await http.PostAsJsonAsync("/api/restores/page-search", req, ct);
            res.EnsureSuccessStatusCode();

            var page = await res.Content.ReadFromJsonAsync<PagedResponse<RestoreRunResponse>>(cancellationToken: ct)
                ?? new PagedResponse<RestoreRunResponse>([], 0, req.PageNumber, req.PageSize);

            return page.ToClient();
        }
    }
}
