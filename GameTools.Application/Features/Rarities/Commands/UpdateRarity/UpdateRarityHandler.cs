using GameTools.Application.Abstractions.WriteStore;
using GameTools.Application.Abstractions.Works;
using GameTools.Application.Features.Rarities.Dtos;
using MediatR;

namespace GameTools.Application.Features.Rarities.Commands.UpdateRarity
{
    public sealed class UpdateRarityHandler(IRarityWriteStore rarityWriteStore, IUnitOfWork uow)
        : IRequestHandler<UpdateRarityCommand, RarityDto>
    {
        public async Task<RarityDto> Handle(UpdateRarityCommand request, CancellationToken ct)
        {
            var rarity = await rarityWriteStore.GetByIdAsync(request.Id, ct)
                         ?? throw new InvalidOperationException("Rarity not found.");

            rarity.SetGrade(request.Grade);
            rarity.SetColorCode(request.NormalizedColorCode);

            await uow.SaveChangesAsync(ct);

            return new RarityDto(rarity.Id, rarity.Grade, rarity.ColorCode);
        }
    }
}
