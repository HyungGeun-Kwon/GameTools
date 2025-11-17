using GameTools.Client.Domain.Rarities;

namespace GameTools.Client.Wpf.Shared.Components.Lookups.Rarities
{
    public static class RarityLookupMappers
    {
        public static RarityOptionModel ToOption(this Rarity r)
            => new(r.Id, r.Grade, r.ColorCode);

        public static IList<RarityOptionModel> ToOptions(this IReadOnlyCollection<Rarity> rarities)
            => rarities.Select(ToOption).ToList();
    }
}
