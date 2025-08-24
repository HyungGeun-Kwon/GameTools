using GameTools.Server.Domain.Auditing;
using GameTools.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Infrastructure.Persistence
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

            modelBuilder.Entity<Item>().HasOne(i => i.Rarity).WithMany(r => r.Items).HasForeignKey(i => i.RarityId).IsRequired();
        }
    }
}
