using GameTools.Domain.Auditing;
using GameTools.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Infrastructure.Persistence
{
    public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Item> Items => Set<Item>();
        public DbSet<Rarity> Rarities => Set<Rarity>();
        public DbSet<ItemAudit> ItemAudits => Set<ItemAudit>();
        public DbSet<RarityAudit> RarityAudits => Set<RarityAudit>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
