using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTools.Domain.Entities;
using GameTools.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Test.Utils
{
    public static class TestDataBase
    {
        public static AppDbContext CreateTestDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase($"app-{Guid.NewGuid()}")
                .AddInterceptors(new InMemoryRowVersionInterceptor())
                .EnableSensitiveDataLogging().Options;

            return new AppDbContext(options);
        }
        public static Rarity SeedRarity(AppDbContext db, string grade = "Common", string colorCode = "#000000")
        {
            var rarity = new Rarity(grade, colorCode);
            db.Set<Rarity>().Add(rarity); 
            db.SaveChanges();
            return rarity;
        }
    }
}
