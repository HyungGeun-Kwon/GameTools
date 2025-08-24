using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTools.Client.Application.Ports;
using GameTools.Client.Application.UseCases.CreateItem;

namespace GameTools.Client.Application.UseCases.BulkInsertItems
{
    public sealed class BulkInsertItemsUseCase(IItemsGateway gateway)
    {
        public Task<BulkInsertItemsOutput> Handle(BulkInsertItemsInput input, CancellationToken ct)
            => gateway.BulkInsertAsync(input, ct);
    }
}
