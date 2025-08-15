using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

            // 버전 불일치 시 : DbUpdateConcurrencyException
            modelBuilder.Entity<Item>().Property<byte[]>("RowVersion").IsRowVersion();
            modelBuilder.Entity<Rarity>().Property<byte[]>("RowVersion").IsRowVersion();
        }
    }
}
