using GameTools.Server.Application.Abstractions.Exceptions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Features.Items.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Queries.GetItemById
{
    public sealed class GetItemByIdHandler(IItemReadStore itemReadStore)
        : IRequestHandler<GetItemByIdQuery, ItemReadModel>
    {
        public async Task<ItemReadModel> Handle(GetItemByIdQuery query, CancellationToken ct)
        {
            var itemReadModel = await itemReadStore.GetByIdAsync(query.Id, ct)
                ?? throw new NotFoundException($"Item '{query.Id}' not found.");

            return itemReadModel;
        }
    }
}
