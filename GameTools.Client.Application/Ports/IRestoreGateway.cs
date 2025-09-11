using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.Models.Restores;
using GameTools.Client.Application.UseCases.Restores.GetRestoresPage;

namespace GameTools.Client.Application.Ports
{
    public interface IRestoreGateway
    {
        Task<PagedOutput<Restore>> GetPageAsync(GetRestoresPageInput input, CancellationToken ct);
    }
}
