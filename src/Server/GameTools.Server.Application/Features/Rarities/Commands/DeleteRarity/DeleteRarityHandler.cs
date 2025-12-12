using GameTools.Server.Application.Abstractions.Exceptions;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.UnitOfWorks;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Commands.DeleteRarity
{
    public sealed class DeleteRarityHandler(
        IRarityWriteStore rarityWriteStore, 
        IUnitOfWork uow)
        : IRequestHandler<DeleteRarityCommand, DeleteRarityResult>
    {
        public async Task<DeleteRarityResult> Handle(DeleteRarityCommand command, CancellationToken ct)
        {
            var rarity = await rarityWriteStore.LoadForUpdateAsync(RarityId.From(command.Spec.Id), ct)
                ?? throw new NotFoundException($"Rarity '{command.Spec.Id}' not found.");

            rarityWriteStore.SetOriginalRowVersion(rarity, command.Spec.RowVersion);
            rarityWriteStore.Remove(rarity);
            
            await uow.SaveChangesAsync(ct);

            return new DeleteRarityResult();
        }
    }
}
