using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.Models.Restores;
using GameTools.Client.Application.Ports;

namespace GameTools.Client.Application.UseCases.Restores.GetRestoreHistoryPage
{
    public sealed class GetRestoreHistoriesPageUseCase(IRestoreGateway gateway)
    {
        public Task<PagedOutput<RestoreHistory>> Handle(GetRestoreHistoriesPageInput input, CancellationToken ct)
            => gateway.GetPageAsync(input, ct);
    }
}
