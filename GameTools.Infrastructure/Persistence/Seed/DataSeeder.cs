using GameTools.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Infrastructure.Persistence.Seed
{
    public class DataSeeder : ISeeder
    {
        public async Task SeedAsync(AppDbContext dbContext, CancellationToken ct = default)
        {
            await SeedRaritiesAsync(dbContext, ct);
            await dbContext.SaveChangesAsync(ct);
            await SeedItemsAsync(dbContext, ct);
            await dbContext.SaveChangesAsync(ct);
        }
        private async Task SeedRaritiesAsync(AppDbContext db, CancellationToken ct)
        {
            if (await db.Rarities.AnyAsync(ct)) return;

            db.Rarities.AddRange(
                new Rarity("Common", "#A0A0A0"),
                new Rarity("Uncommon", "#1EFF00"),
                new Rarity("Rare", "#0070FF"),
                new Rarity("Epic", "#A335EE"),
                new Rarity("Legendary", "#FF8000")
            );
        }

        private async Task SeedItemsAsync(AppDbContext db, CancellationToken ct)
        {
            if (await db.Items.AnyAsync(ct)) return;

            var common = await db.Rarities.AsNoTracking().SingleAsync(r => r.Grade == "Common", ct);
            var uncommon = await db.Rarities.AsNoTracking().SingleAsync(r => r.Grade == "Uncommon", ct);
            var rare = await db.Rarities.AsNoTracking().SingleAsync(r => r.Grade == "Rare", ct);
            var legendary = await db.Rarities.AsNoTracking().SingleAsync(r => r.Grade == "Legendary", ct);

            db.Items.AddRange(
                new Item("Small HP Potion", 100, common.Id, "HP를 100 회복합니다."),
                new Item("Small MP Potion", 100, common.Id, "MP를 100 회복합니다."),
                new Item("Medium HP Potion", 700, uncommon.Id, "HP를 500 회복합니다."),
                new Item("Medium MP Potion", 700, uncommon.Id, "MP를 500 회복합니다."),
                new Item("Large HP Potion", 4000, rare.Id, "HP를 2000 회복합니다."),
                new Item("Large MP Potion", 4000, rare.Id, "MP를 2000 회복합니다."),
                new Item("Phoenix Feather", 50000, legendary.Id, "아군을 부활시킵니다.")
            );
        }
    }
}
