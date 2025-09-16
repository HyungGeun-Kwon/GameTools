using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.Works;
using GameTools.Server.Application.Common.Results;
using GameTools.Server.Application.Features.Rarities.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Application.Features.Rarities.Commands.UpdateRarity
{
    public sealed class UpdateRarityHandler(IRarityWriteStore rarityWriteStore, IUnitOfWork uow)
        : IRequestHandler<UpdateRarityCommand, UpdateRarityResult>
    {
        public async Task<UpdateRarityResult> Handle(UpdateRarityCommand request, CancellationToken ct)
        {
            var rarity = await rarityWriteStore.GetByIdAsync(request.Payload.Id, ct);
            if (rarity == null)
                return new UpdateRarityResult(WriteStatusCode.NotFound, null);

            // 감시하고있는  버전 업데이트
            rarityWriteStore.SetOriginalRowVersion(rarity, request.Payload.RowVersion);

            rarity.SetGrade(request.Payload.Grade);
            rarity.SetColorCode(request.NormalizedColorCode);

            try
            {
                await uow.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await rarityWriteStore.GetByIdAsync(request.Payload.Id, ct);
                return exists == null ? new(WriteStatusCode.NotFound, null) : new(WriteStatusCode.VersionMismatch, null);
            }

            return new UpdateRarityResult(
                WriteStatusCode.Success, 
                new RarityReadModel(rarity.Id, rarity.Grade, rarity.ColorCode, rarity.RowVersion));
        }
    }
}
