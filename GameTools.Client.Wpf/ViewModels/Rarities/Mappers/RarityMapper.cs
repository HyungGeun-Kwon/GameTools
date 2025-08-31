using GameTools.Client.Application.UseCases.Rarities.CreateRarity;
using GameTools.Client.Application.UseCases.Rarities.DeleteRarity;
using GameTools.Client.Application.UseCases.Rarities.UpdateRarity;
using GameTools.Client.Domain.Rarities;
using GameTools.Client.Wpf.ViewModels.Rarities.Contracts;

namespace GameTools.Client.Wpf.ViewModels.Rarities.Mappers
{
    public static class RarityMapper
    {
        public static RarityEditModel ToEditModel(this Rarity rarity)
            => new(rarity.Id, rarity.Grade, rarity.ColorCode, rarity.RowVersionBase64);

        public static CreateRarityInput ToCreateRarityInput(this RarityEditModel rarityEditModel)
        {
            if (rarityEditModel.Id is not null || rarityEditModel.RowVersionBase64 is not null)
            {
                throw new InvalidOperationException("CreateRarityInput requires Id or RowVersionBase64 to be null.");
            }
            return new(rarityEditModel.Grade, rarityEditModel.ColorCode);
        }

        public static DeleteRarityInput ToDeleteRarityInput(this RarityEditModel rarityEditModel)
        {
            if (rarityEditModel.Id is null)
            {
                throw new ArgumentNullException(nameof(rarityEditModel), "DeleteRarityInput requires a valid Id.");
            }
            if (string.IsNullOrEmpty(rarityEditModel.RowVersionBase64))
            {
                throw new ArgumentNullException(nameof(rarityEditModel), "DeleteRarityInput requires a valid RowVersionBase64.");
            }
            return new((byte)rarityEditModel.Id, rarityEditModel.RowVersionBase64);
        }   

        public static UpdateRarityInput ToUpdateRarityInput(this RarityEditModel rarityEditModel)
        {
            if (rarityEditModel.Id is null)
            {
                throw new ArgumentNullException(nameof(rarityEditModel), "UpdateRarityInput requires a valid Id.");
            }
            if (string.IsNullOrEmpty(rarityEditModel.RowVersionBase64))
            {
                throw new ArgumentNullException(nameof(rarityEditModel), "UpdateRarityInput requires a valid RowVersionBase64.");
            }

            return new((byte)rarityEditModel.Id, rarityEditModel.Grade, rarityEditModel.ColorCode, rarityEditModel.RowVersionBase64);
        }

        public static IEnumerable<RarityEditModel> ToEditModels(this IEnumerable<Rarity>? rarities)
            => (rarities ?? []).Select(r => r.ToEditModel());
    }
}
