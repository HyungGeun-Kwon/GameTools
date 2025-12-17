using GameTools.Server.Application.Abstractions.Exceptions;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Abstractions.UnitOfWorks;
using GameTools.Server.Domain.Catalog.Rarities.Policies;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using MediatR;

namespace GameTools.Server.Application.Catalog.Rarities.Commands.UpdateRarity
{
    public sealed class UpdateRarityHandler(
        IRarityWriteStore rarityWriteStore,
        IRarityGradeUniquenessPolicy rarityGradeUniquenessPolicy,
        IUnitOfWork uow)
        : IRequestHandler<UpdateRarityCommand, UpdateRarityResult>
    {
        public async Task<UpdateRarityResult> Handle(UpdateRarityCommand command, CancellationToken ct)
        {
            var rarity = await rarityWriteStore.LoadForUpdateAsync(RarityId.From(command.Spec.Id), ct)
                ?? throw new NotFoundException($"Rarity '{command.Spec.Id}' not found.");

            var newGrade = new RarityGrade(command.Spec.Grade);
            var newColorCode = new RarityColorCode(command.Spec.NormalizedColorCode);

            if (rarity.Grade != newGrade)
                await rarityGradeUniquenessPolicy.EnsureUniqueAsync(newGrade, ct);

            rarityWriteStore.SetOriginalRowVersion(rarity, command.Spec.RowVersion);

            rarity.ChangeGrade(newGrade);
            rarity.ChangeColor(newColorCode);

            await uow.SaveChangesAsync(ct);

            var rowVersion = rarityWriteStore.GetRowVersion(rarity);

            return new UpdateRarityResult(rarity.Id.Value, rowVersion);
        }
    }
}
