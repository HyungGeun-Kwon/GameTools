using GameTools.Server.Domain.Features.Items.Factories;
using GameTools.Server.Domain.Features.Items.ValueObjects;
using GameTools.Server.Domain.Features.Rarities.Factories;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence.Catalog.Seed
{
    public class DataSeeder(AppDbContext dbContext, IItemFactory itemFactory, IRarityFactory rarityFactory) : ISeeder
    {
        public async Task SeedAsync(CancellationToken ct = default)
        {
            await SeedRaritiesAsync(ct);
            await dbContext.SaveChangesAsync(ct);
            await SeedItemsAsync(ct);
            await dbContext.SaveChangesAsync(ct);
        }
        private async Task SeedRaritiesAsync(CancellationToken ct)
        {
            if (await dbContext.Rarities.AnyAsync(ct)) return;

            dbContext.Rarities.AddRange(
                await rarityFactory.CreateAsync(new RarityGrade("Common"), new RarityColorCode("#A0A0A0"), ct),
                await rarityFactory.CreateAsync(new RarityGrade("Uncommon"), new RarityColorCode("#1EFF00"), ct),
                await rarityFactory.CreateAsync(new RarityGrade("Rare"), new RarityColorCode("#0070FF"), ct),
                await rarityFactory.CreateAsync(new RarityGrade("Epic"), new RarityColorCode("#A335EE"), ct),
                await rarityFactory.CreateAsync(new RarityGrade("Legendary"), new RarityColorCode("#FF8000"), ct)
            );
        }

        private async Task SeedItemsAsync(CancellationToken ct)
        {
            if (await dbContext.Items.AnyAsync(ct)) return;

            var common = await dbContext.Rarities.AsNoTracking().SingleAsync(r => r.Grade == new RarityGrade("Common"), ct);
            var uncommon = await dbContext.Rarities.AsNoTracking().SingleAsync(r => r.Grade == new RarityGrade("Uncommon"), ct);
            var rare = await dbContext.Rarities.AsNoTracking().SingleAsync(r => r.Grade == new RarityGrade("Rare"), ct);
            var legendary = await dbContext.Rarities.AsNoTracking().SingleAsync(r => r.Grade == new RarityGrade("Legendary"), ct);

            dbContext.Items.AddRange(
                await itemFactory.CreateAsync(new ItemName("Small HP Potion"), new ItemPrice(100), new ItemDescription("HP를 100 회복합니다."), RarityId.From(common.Id.Value), ct),
                await itemFactory.CreateAsync(new ItemName("Small MP Potion"), new ItemPrice(100), new ItemDescription("MP를 100 회복합니다."), RarityId.From(common.Id.Value), ct),
                await itemFactory.CreateAsync(new ItemName("Medium HP Potion"), new ItemPrice(700), new ItemDescription("HP를 500 회복합니다."), RarityId.From(uncommon.Id.Value), ct),
                await itemFactory.CreateAsync(new ItemName("Medium MP Potion"), new ItemPrice(700), new ItemDescription("MP를 500 회복합니다."), RarityId.From(uncommon.Id.Value), ct),
                await itemFactory.CreateAsync(new ItemName("Large HP Potion"), new ItemPrice(4000), new ItemDescription("HP를 2000 회복합니다."), RarityId.From(rare.Id.Value), ct),
                await itemFactory.CreateAsync(new ItemName("Large MP Potion"), new ItemPrice(4000), new ItemDescription("MP를 2000 회복합니다."), RarityId.From(rare.Id.Value), ct),
                await itemFactory.CreateAsync(new ItemName("Phoenix Feather"), new ItemPrice(50000), new ItemDescription("아군을 부활시킵니다."), RarityId.From(legendary.Id.Value), ct)
            );
        }
    }
}
