using GameTools.Client.Application.UseCases.Rarities.CreateRarity;
using GameTools.Client.Application.UseCases.Rarities.UpdateRarity;
using GameTools.Client.Domain.Rarities;
using GameTools.Contracts.Rarities.Common;
using GameTools.Contracts.Rarities.CreateRarity;
using GameTools.Contracts.Rarities.GetAllRarities;
using GameTools.Contracts.Rarities.UpdateRarity;

namespace GameTools.Client.Infrastructure.Mapper
{
    public static class RarityContractMapper
    {
        public static CreateRarityRequest ToContract(this CreateRarityInput input)
            => new(input.Grade, input.ColorCode);

        public static UpdateRarityRequest ToContract(this UpdateRarityInput input)
            => new(input.Id, input.Grade, input.ColorCode, input.RowVersionBase64);

        public static Rarity ToClient(this RarityResponse r)
            => new(r.Id, r.Grade, r.ColorCode, r.RowVersionBase64);

        public static IReadOnlyList<Rarity> ToClient(this AllRarityResponse r)
            => r.Rarities.Select(ToClient).ToList();

    }
}
