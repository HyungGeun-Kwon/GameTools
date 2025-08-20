using GameTools.Application.Abstractions.Works;
using GameTools.Application.Features.Items.Dtos;
using MediatR;
using GameTools.Application.Abstractions.Stores.WriteStore;

namespace GameTools.Application.Features.Items.Commands.UpdateItem
{
    public sealed class UpdateItemHandler(IItemWriteStore itemWriteStore, IRarityWriteStore rarityRepo, IUnitOfWork uow)
        : IRequestHandler<UpdateItemCommand, ItemDto>
    {
        public async Task<ItemDto> Handle(UpdateItemCommand request, CancellationToken ct)
        {
            var item = await itemWriteStore.GetByIdAsync(request.ItemUpdateDto.Id, ct)
                       ?? throw new InvalidOperationException("Item not found.");

            // 감시하고있는  버전 업데이트
            itemWriteStore.SetOriginalRowVersion(item, request.ItemUpdateDto.RowVersionBase64);

            // 변경
            item.SetName(request.ItemUpdateDto.Name);
            item.SetPrice(request.ItemUpdateDto.Price);
            item.SetDescription(request.ItemUpdateDto.Description);

            if (item.RarityId != request.ItemUpdateDto.RarityId)
            {
                var rarity = await rarityRepo.GetByIdAsync(request.ItemUpdateDto.RarityId, ct)
                             ?? throw new InvalidOperationException("Rarity not found.");
                item.SetRarity(rarity);
            }

            await uow.SaveChangesAsync(ct);

            return new ItemDto(
                item.Id,
                item.Name,
                item.Price,
                item.Description,
                item.RarityId,
                item.Rarity.Grade,
                item.Rarity.ColorCode,
                Convert.ToBase64String(item.RowVersion));
        }
    }
}
