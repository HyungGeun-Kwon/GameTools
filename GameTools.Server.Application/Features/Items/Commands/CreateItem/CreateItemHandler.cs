using GameTools.Server.Application.Abstractions.Works;
using GameTools.Server.Domain.Entities;
using MediatR;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Features.Items.Models;

namespace GameTools.Server.Application.Features.Items.Commands.CreateItem
{
    public sealed class CreateItemHandler(IItemWriteStore itemWriteStore, IRarityWriteStore rarityRepo, IUnitOfWork uow)
        : IRequestHandler<CreateItemCommand, ItemReadModel>
    {
        public async Task<ItemReadModel> Handle(CreateItemCommand request, CancellationToken ct)
        {
            var rarity = await rarityRepo.GetByIdAsync(request.Payload.RarityId, ct)
                ?? throw new InvalidOperationException("Rarity not found.");

            var item = new Item(
                request.Payload.Name,
                request.Payload.Price,
                rarity,
                request.Payload.Description);

            await itemWriteStore.AddAsync(item, ct);
            await uow.SaveChangesAsync(ct);

            return new ItemReadModel(
                item.Id,
                item.Name,
                item.Price,
                item.Description,
                item.RarityId,
                item.Rarity.Grade,
                item.Rarity.ColorCode,
                item.RowVersion);
        }
    }
}
