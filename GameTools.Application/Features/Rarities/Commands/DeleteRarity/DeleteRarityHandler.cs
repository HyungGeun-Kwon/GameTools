using GameTools.Application.Abstractions.Stores.WriteStore;
using GameTools.Application.Abstractions.Works;
using GameTools.Domain.Entities;
using MediatR;

namespace GameTools.Application.Features.Rarities.Commands.DeleteRarity
{
    public sealed class DeleteRarityHandler(IRarityWriteStore rarityWriteStore, IUnitOfWork uow)
        : IRequestHandler<DeleteRarityCommand>
    {
        public async Task Handle(DeleteRarityCommand request, CancellationToken ct)
        {
            var rarity = await rarityWriteStore.GetByIdAsync(request.RarityDeleteDto.Id, ct)
                         ?? throw new InvalidOperationException("Rarity not found.");

            rarityWriteStore.SetOriginalRowVersion(rarity, request.RarityDeleteDto.RowVersionBase64);

            rarityWriteStore.Remove(rarity);
            await uow.SaveChangesAsync(ct);
        }
    }
}
