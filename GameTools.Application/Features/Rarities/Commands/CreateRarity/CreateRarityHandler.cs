using GameTools.Application.Abstractions.WriteStore;
using GameTools.Application.Abstractions.Works;
using GameTools.Application.Features.Rarities.Dtos;
using GameTools.Domain.Entities;
using MediatR;

namespace GameTools.Application.Features.Rarities.Commands.CreateRarity
{
    public sealed class CreateRarityHandler(IRarityWriteStore rarityWriteStore, IUnitOfWork uow) : IRequestHandler<CreateRarityCommand, RarityDto>
    {
        public async Task<RarityDto> Handle(CreateRarityCommand request, CancellationToken ct)
        {
            var rarity = new Rarity(request.Grade, request.NormalizedColorCode);
            await rarityWriteStore.AddAsync(rarity, ct);
            await uow.SaveChangesAsync(ct);
            return new RarityDto(rarity.Id, rarity.Grade, rarity.ColorCode);
        }
    }
}
