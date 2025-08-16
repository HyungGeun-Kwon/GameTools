using GameTools.Application.Abstractions.WriteStore;
using GameTools.Application.Abstractions.Works;
using GameTools.Application.Features.Items.Dtos;
using GameTools.Domain.Entities;
using MediatR;

namespace GameTools.Application.Features.Items.Commands.CreateItem
{
    public sealed class CreateItemHandler(IItemWriteStore itemWriteStore, IRarityWriteStore rarityRepo, IUnitOfWork uow)
        : IRequestHandler<CreateItemCommand, ItemDto>
    {
        public async Task<ItemDto> Handle(CreateItemCommand request, CancellationToken ct)
        {
            var rarity = await rarityRepo.GetByIdAsync(request.RarityId, ct)
                         ?? throw new InvalidOperationException("Rarity not found.");

            var item = new Item(request.Name, request.Price, rarity, request.Description);

            await itemWriteStore.AddAsync(item, ct);
            await uow.SaveChangesAsync(ct);

            return new ItemDto(item.Id, item.Name, item.Price, item.Description, item.RarityId, item.Rarity.Grade, item.Rarity.ColorCode);
        }
    }
}
