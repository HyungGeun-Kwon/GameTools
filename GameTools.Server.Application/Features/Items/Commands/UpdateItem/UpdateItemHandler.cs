using GameTools.Server.Application.Abstractions.Works;
using MediatR;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Features.Items.Models;

namespace GameTools.Server.Application.Features.Items.Commands.UpdateItem
{
    public sealed class UpdateItemHandler(IItemWriteStore itemWriteStore, IRarityWriteStore rarityRepo, IUnitOfWork uow)
        : IRequestHandler<UpdateItemCommand, ItemReadModel>
    {
        public async Task<ItemReadModel> Handle(UpdateItemCommand request, CancellationToken ct)
        {
            var item = await itemWriteStore.GetByIdAsync(request.Payload.Id, ct)
                       ?? throw new InvalidOperationException("Item not found.");

            // 감시하고있는  버전 업데이트
            itemWriteStore.SetOriginalRowVersion(item, request.Payload.RowVersion);

            // 변경
            item.SetName(request.Payload.Name);
            item.SetPrice(request.Payload.Price);
            item.SetDescription(request.Payload.Description);

            if (item.RarityId != request.Payload.RarityId)
            {
                var rarity = await rarityRepo.GetByIdAsync(request.Payload.RarityId, ct)
                    ?? throw new InvalidOperationException("Rarity not found.");
                item.SetRarity(rarity);
            }

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
