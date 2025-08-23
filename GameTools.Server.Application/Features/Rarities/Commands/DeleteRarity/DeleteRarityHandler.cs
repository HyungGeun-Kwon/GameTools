using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.Works;
using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Commands.DeleteRarity
{
    public sealed class DeleteRarityHandler(IRarityWriteStore rarityWriteStore, IUnitOfWork uow)
        : IRequestHandler<DeleteRarityCommand>
    {
        public async Task Handle(DeleteRarityCommand request, CancellationToken ct)
        {
            var rarity = await rarityWriteStore.GetByIdAsync(request.Payload.Id, ct)
                         ?? throw new InvalidOperationException("Rarity not found.");

            rarityWriteStore.SetOriginalRowVersion(rarity, request.Payload.RowVersion);

            rarityWriteStore.Remove(rarity);
            await uow.SaveChangesAsync(ct);
        }
    }
}
