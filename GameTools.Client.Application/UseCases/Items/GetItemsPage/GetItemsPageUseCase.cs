using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.Ports;
using GameTools.Client.Domain.Items;

namespace GameTools.Client.Application.UseCases.Items.GetItemsPage
{
    public sealed class GetItemsPageUseCase(IItemsGateway gateway)
    {
        public Task<PagedOutput<Item>> Handle(GetItemsPageInput input, CancellationToken ct)
            => gateway.GetPageAsync(input, ct);
    }
}
