using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.Models.Restores;
using GameTools.Client.Application.UseCases.Restores.GetRestoreHistoryPage;

namespace GameTools.Client.Application.Ports
{
    public interface IRestoreGateway
    {
        Task<PagedOutput<RestoreHistory>> GetPageAsync(GetRestoreHistoriesPageInput input, CancellationToken ct);
    }
}
