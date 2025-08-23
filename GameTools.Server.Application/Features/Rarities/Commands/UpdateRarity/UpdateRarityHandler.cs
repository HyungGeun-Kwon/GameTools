using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.Works;
using GameTools.Server.Application.Features.Rarities.Models;
using MediatR;

namespace GameTools.Server.Application.Features.Rarities.Commands.UpdateRarity
{
    public sealed class UpdateRarityHandler(IRarityWriteStore rarityWriteStore, IUnitOfWork uow)
        : IRequestHandler<UpdateRarityCommand, RarityReadModel>
    {
        public async Task<RarityReadModel> Handle(UpdateRarityCommand request, CancellationToken ct)
        {
            var rarity = await rarityWriteStore.GetByIdAsync(request.Payload.Id, ct)
                         ?? throw new InvalidOperationException("Rarity not found.");

            // 감시하고있는  버전 업데이트
            rarityWriteStore.SetOriginalRowVersion(rarity, request.Payload.RowVersion);

            rarity.SetGrade(request.Payload.Grade);
            rarity.SetColorCode(request.NormalizedColorCode);

            await uow.SaveChangesAsync(ct);

            return new RarityReadModel(rarity.Id, rarity.Grade, rarity.ColorCode, rarity.RowVersion);
        }
    }
}
