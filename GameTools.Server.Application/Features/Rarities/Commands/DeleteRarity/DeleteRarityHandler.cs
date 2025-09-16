using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.Works;
using GameTools.Server.Application.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Application.Features.Rarities.Commands.DeleteRarity
{
    public sealed class DeleteRarityHandler(IRarityWriteStore rarityWriteStore, IUnitOfWork uow)
        : IRequestHandler<DeleteRarityCommand, WriteStatusCode>
    {
        public async Task<WriteStatusCode> Handle(DeleteRarityCommand request, CancellationToken ct)
        {
            var rarity = await rarityWriteStore.GetByIdAsync(request.Payload.Id, ct);
            if (rarity == null)
                return WriteStatusCode.NotFound;
            rarityWriteStore.SetOriginalRowVersion(rarity, request.Payload.RowVersion);

            rarityWriteStore.Remove(rarity);
            
            try
            {
                await uow.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await rarityWriteStore.GetByIdAsync(request.Payload.Id, ct);
                return exists == null ? WriteStatusCode.NotFound : WriteStatusCode.VersionMismatch;
            }

            return WriteStatusCode.Success;
        }
    }
}
