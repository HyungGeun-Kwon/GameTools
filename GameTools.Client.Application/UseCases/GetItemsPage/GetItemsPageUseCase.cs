using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTools.Client.Application.Common.Paging;
using GameTools.Client.Application.Ports;
using GameTools.Client.Domain.Items;

namespace GameTools.Client.Application.UseCases.GetItemsPage
{
    public sealed class GetItemsPageUseCase(IItemsGateway gateway)
    {
        public Task<PagedResult<Item>> Handle(GetItemsPageInput input, CancellationToken ct)
            => gateway.GetPageAsync(input, ct);
    }
}
