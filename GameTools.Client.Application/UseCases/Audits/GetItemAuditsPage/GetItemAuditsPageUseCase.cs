using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.Models.Audits;
using GameTools.Client.Application.Ports;

namespace GameTools.Client.Application.UseCases.Audits.GetItemAuditsPage
{
    public sealed class GetItemAuditsPageUseCase(IAuditsGateway gateway)
    {
        public Task<PagedOutput<ItemAudit>> Handle(GetItemAuditsPageInput input, CancellationToken ct)
            => gateway.GetPageAsync(input, ct);
    }
}
