using GameTools.Application.Abstractions.Works;
using GameTools.Application.Abstractions.WriteStore;
using GameTools.Application.Features.Rarities.Dtos;
using GameTools.Domain.Entities;
using MediatR;

namespace GameTools.Application.Features.Rarities.Commands.UpdateRarity
{
    public sealed class UpdateRarityHandler(IRarityWriteStore rarityWriteStore, IUnitOfWork uow)
        : IRequestHandler<UpdateRarityCommand, RarityDto>
    {
        public async Task<RarityDto> Handle(UpdateRarityCommand request, CancellationToken ct)
        {
            var rarity = await rarityWriteStore.GetByIdAsync(request.RarityUpdateDto.Id, ct)
                         ?? throw new InvalidOperationException("Rarity not found.");

            // 감시하고있는  버전 업데이트
            rarityWriteStore.SetOriginalRowVersion(rarity, request.RarityUpdateDto.RowVersionBase64);

            rarity.SetGrade(request.RarityUpdateDto.Grade);
            rarity.SetColorCode(request.NormalizedColorCode);

            await uow.SaveChangesAsync(ct);

            return new RarityDto(rarity.Id, rarity.Grade, rarity.ColorCode, Convert.ToBase64String(rarity.RowVersion));
        }
    }
}
