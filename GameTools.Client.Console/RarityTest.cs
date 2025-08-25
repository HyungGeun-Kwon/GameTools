using System.Diagnostics;
using GameTools.Client.Application.UseCases.Rarities.CreateRarity;
using GameTools.Client.Application.UseCases.Rarities.DeleteRarity;
using GameTools.Client.Application.UseCases.Rarities.GetAllRarities;
using GameTools.Client.Application.UseCases.Rarities.GetRarityById;
using GameTools.Client.Application.UseCases.Rarities.UpdateRarity;
using GameTools.Client.Domain.Rarities;

namespace GameTools.Client.Console
{
    public class RarityTest(
        GetRarityByIdUseCase getRarityById,
        GetAllRaritiesUseCase getAllRarities,
        CreateRarityUseCase createRarity,
        UpdateRarityUseCase updateRarity,
        DeleteRarityUseCase deleteRarity)
    {
        public async Task Run()
        {
            Rarity createdRaity = await createRarity.Handle(
                new CreateRarityInput($"TestRarity_{Stopwatch.GetTimestamp()}", "#123456"), CancellationToken.None);

            Rarity? getRarity = await getRarityById.Handle(createdRaity.Id, CancellationToken.None);

            Rarity updatedRarity = await updateRarity.Handle(
                new UpdateRarityInput(createdRaity.Id, createdRaity.Grade + "_updated", "#654321", createdRaity.RowVersionBase64),
                CancellationToken.None);

            IReadOnlyList<Rarity> getAll = await getAllRarities.Handle(CancellationToken.None);

            try
            {
                await deleteRarity.Handle(new DeleteRarityInput(createdRaity.Id, createdRaity.RowVersionBase64), CancellationToken.None);
                throw new Exception("Error : Row Version이 다르지만 Update 성공.");
            }
            catch { }

            await deleteRarity.Handle(new DeleteRarityInput(updatedRarity.Id, updatedRarity.RowVersionBase64), CancellationToken.None);
        }
    }
}
