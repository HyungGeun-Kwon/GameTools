using MediatR;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using GameTools.Server.Domain.Catalog.Rarities.Factories;
using GameTools.Server.Application.Abstractions.UnitOfWorks;

namespace GameTools.Server.Application.Catalog.Rarities.Commands.CreateRarity
{
    public sealed class CreateRarityHandler(
        IRarityWriteStore rarityWriteStore,
        IRarityFactory rarityFactory,
        IUnitOfWork uow)
        : IRequestHandler<CreateRarityCommand, CreateRarityResult>
    {
        public async Task<CreateRarityResult> Handle(CreateRarityCommand command, CancellationToken ct)
        {
            var grade = new RarityGrade(command.Spec.Grade);
            var colorCode = new RarityColorCode(command.Spec.NormalizedColorCode);

            var rarity = await rarityFactory.CreateAsync(grade, colorCode, ct);

            await rarityWriteStore.AddAsync(rarity, ct);

            await uow.SaveChangesAsync(ct);

            var rowVersion = rarityWriteStore.GetRowVersion(rarity);

            return new CreateRarityResult(rarity.Id.Value, rowVersion);
        }
    }
}
