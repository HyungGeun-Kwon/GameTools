using GameTools.Server.Application.Abstractions.Works;
using GameTools.Server.Domain.Entities;
using MediatR;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Features.Rarities.Models;

namespace GameTools.Server.Application.Features.Rarities.Commands.CreateRarity
{
    public sealed class CreateRarityHandler(IRarityWriteStore rarityWriteStore, IUnitOfWork uow) : IRequestHandler<CreateRarityCommand, RarityReadModel>
    {
        public async Task<RarityReadModel> Handle(CreateRarityCommand request, CancellationToken ct)
        {
            var rarity = new Rarity(request.Payload.Grade, request.NormalizedColorCode);
            await rarityWriteStore.AddAsync(rarity, ct);
            await uow.SaveChangesAsync(ct);
            return new RarityReadModel(rarity.Id, rarity.Grade, rarity.ColorCode, rarity.RowVersion);
        }
    }
}
