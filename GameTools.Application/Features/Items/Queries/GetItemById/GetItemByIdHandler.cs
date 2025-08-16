using GameTools.Application.Abstractions.ReadStore;
using GameTools.Application.Features.Items.Dtos;
using MediatR;

namespace GameTools.Application.Features.Items.Queries.GetItemById
{
    public sealed class GetItemByIdHandler(IItemReadStore itemReadStore) : IRequestHandler<GetItemByIdQuery, ItemDto?>
    {
        public async Task<ItemDto?> Handle(GetItemByIdQuery request, CancellationToken ct)
            => await itemReadStore.GetByIdAsync(request.Id, ct);
    }
}
