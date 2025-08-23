using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Features.Items.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Queries.GetItemById
{
    public sealed class GetItemByIdHandler(IItemReadStore itemReadStore) : IRequestHandler<GetItemByIdQuery, ItemReadModel?>
    {
        public async Task<ItemReadModel?> Handle(GetItemByIdQuery request, CancellationToken ct)
            => await itemReadStore.GetByIdAsync(request.Id, ct);
    }
}
