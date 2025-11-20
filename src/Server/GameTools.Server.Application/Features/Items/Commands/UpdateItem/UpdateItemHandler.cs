using GameTools.Server.Application.Abstractions.Exceptions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.UnitOfWorks;
using GameTools.Server.Domain.Features.Items.Policies;
using GameTools.Server.Domain.Features.Items.ValueObjects;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.UpdateItem
{
    public sealed class UpdateItemHandler(
        IItemWriteStore itemWriteStore,
        IRarityReadStore rarityReadStore,
        IItemNameUniquenessPolicy itemNameUniquenessPolicy,
        IUnitOfWork uow)
        : IRequestHandler<UpdateItemCommand, UpdateItemResult>
    {
        public async Task<UpdateItemResult> Handle(UpdateItemCommand command, CancellationToken ct)
        {
            var item = await itemWriteStore.LoadForUpdateAsync(command.Spec.Id, ct)
                ?? throw new NotFoundException($"Item '{command.Spec.Id}' not found.");

            _ = await rarityReadStore.GetByIdAsync(command.Spec.RarityId, ct)
                ?? throw new NotFoundException($"Rarity '{command.Spec.RarityId}' not found.");

            var newName = new ItemName(command.Spec.Name);

            // 이름은 Unique
            if (item.Name != newName)
                await itemNameUniquenessPolicy.EnsureUniqueAsync(newName, ct);

            // 감시하고있는  버전 업데이트
            itemWriteStore.SetOriginalRowVersion(item, command.Spec.RowVersion);

            // 변경
            item.Rename(newName);
            item.ChangePrice(new ItemPrice(command.Spec.Price));
            item.ChangeDescription(new ItemDescription(command.Spec.Description));
            item.ChangeRarity(RarityId.From(command.Spec.RarityId));

            await uow.SaveChangesAsync(ct);

            var rowVersion = itemWriteStore.GetRowVersion(item);

            return new UpdateItemResult(item.Id.Value, rowVersion);
        }
    }
}
