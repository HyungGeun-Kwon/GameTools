using GameTools.Application.Abstractions.WriteStore;
using GameTools.Application.Abstractions.Works;
using GameTools.Application.Features.Items.Dtos;
using MediatR;

namespace GameTools.Application.Features.Items.Commands.UpdateItem
{
    public sealed class UpdateItemHandler(IItemWriteStore itemWriteStore, IRarityWriteStore rarityRepo, IUnitOfWork uow)
        : IRequestHandler<UpdateItemCommand, ItemDto>
    {
        public async Task<ItemDto> Handle(UpdateItemCommand request, CancellationToken ct)
        {
            var item = await itemWriteStore.GetByIdAsync(request.Id, ct)
                       ?? throw new InvalidOperationException("Item not found.");

            // 변경
            item.SetName(request.Name);
            item.SetPrice(request.Price);
            item.SetDescription(request.Description);

            if (item.RarityId != request.RarityId)
            {
                var rarity = await rarityRepo.GetByIdAsync(request.RarityId, ct)
                             ?? throw new InvalidOperationException("Rarity not found.");
                item.SetRarity(rarity);
            }

            await uow.SaveChangesAsync(ct);

            return new ItemDto(item.Id, item.Name, item.Price, item.Description, item.RarityId, item.Rarity.Grade, item.Rarity.ColorCode);
        }
    }
}
