using GameTools.Server.Application.Abstractions.Exceptions;
using GameTools.Server.Application.Abstractions.Stores.ReadStore;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.UnitOfWorks;
using GameTools.Server.Domain.Features.Items.Factories;
using GameTools.Server.Domain.Features.Items.ValueObjects;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using MediatR;

namespace GameTools.Server.Application.Features.Items.Commands.CreateItem
{
    public sealed class CreateItemHandler(
        IItemWriteStore itemWriteStore, 
        IRarityReadStore rarityReadStore,
        IItemFactory itemFactory,
        IUnitOfWork uow) 
        : IRequestHandler<CreateItemCommand, CreateItemResult>
    {
        public async Task<CreateItemResult> Handle(CreateItemCommand command, CancellationToken ct)
        {
            var rarity = await rarityReadStore.GetByIdAsync(command.Spec.RarityId, ct)
                ?? throw new NotFoundException($"Rarity '{command.Spec.RarityId}' not found.");

            var item = await itemFactory.CreateAsync(
                new ItemName(command.Spec.Name),
                new ItemPrice(command.Spec.Price),
                new ItemDescription(command.Spec.Description),
                RarityId.From(rarity.Id), ct);

            await itemWriteStore.AddAsync(item, ct);
            await uow.SaveChangesAsync(ct);

            var rowVersion = itemWriteStore.GetRowVersion(item);

            return new CreateItemResult(
                item.Id.Value,
                rowVersion);
        }
    }
}
