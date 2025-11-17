using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Features.Items.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Queries.GetItemById
{
    public sealed class GetItemByIdHandler(IItemReadStore itemReadStore) : IRequestHandler<GetItemByIdQuery, ItemReadModel?>
    {
        public Task<ItemReadModel?> Handle(GetItemByIdQuery request, CancellationToken ct)
            =>  itemReadStore.GetByIdAsync(request.Id, ct);
    }
}
