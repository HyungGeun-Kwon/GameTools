using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.Models.Restores;
using GameTools.Client.Application.Ports;

namespace GameTools.Client.Application.UseCases.Restores.GetRestoresPage
{
    public sealed class GetRestoresPageUseCase(IRestoreGateway gateway)
    {
        public Task<PagedOutput<Restore>> Handle(GetRestoresPageInput input, CancellationToken ct)
            => gateway.GetPageAsync(input, ct);
    }
}
