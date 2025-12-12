using System.Data;
using GameTools.Server.Application.Abstractions.Users;
using GameTools.Server.Domain.Features.Items.Entities;
using GameTools.Server.Domain.Features.Rarities.Entities;
using GameTools.Server.Infrastructure.Persistence.Auditing.Models;
using GameTools.Server.Infrastructure.Persistence.Operations.Restores.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GameTools.Server.Infrastructure.Persistence
{
    public sealed class AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser) : DbContext(options)
    {
        public DbSet<Item> Items => Set<Item>();
        public DbSet<Rarity> Rarities => Set<Rarity>();

        public DbSet<ItemAudit> ItemAudits => Set<ItemAudit>();
        public DbSet<RarityAudit> RarityAudits => Set<RarityAudit>();

        public DbSet<RestoreHistory> ItemRestoreHistories => Set<RestoreHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            GuardAuditTablesAreNotModified();

            if (!Database.IsRelational())
                return base.SaveChanges();

            var connection = Database.GetDbConnection();
            var wasOpen = connection.State == ConnectionState.Open;

            if (!wasOpen)
                connection.Open();

            try
            {
                SetCurrentUser();
                return base.SaveChanges();
            }
            finally
            {
                if (!wasOpen)
                    connection.Close();
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            GuardAuditTablesAreNotModified();

            if (!Database.IsRelational())
                return await base.SaveChangesAsync(ct);

            var connection = Database.GetDbConnection();
            var wasOpen = connection.State == ConnectionState.Open;

            if (!wasOpen)
                await connection.OpenAsync(ct);

            try
            {
                await SetCurrentUserAsync(ct);
                return await base.SaveChangesAsync(ct);
            }
            finally
            {
                if (!wasOpen)
                    connection.Close();
            }
        }
        private void SetCurrentUser()
        {
            var user = currentUser.UserIdOrName ?? "unknown";

            Database.ExecuteSqlInterpolated(
                $"EXEC sys.sp_set_session_context @key=N'CurrentUser', @value={user}");
        }
        private Task SetCurrentUserAsync(CancellationToken ct)
        {
            var user = currentUser.UserIdOrName ?? "unknown";

            return Database.ExecuteSqlInterpolatedAsync(
                $"EXEC sys.sp_set_session_context @key=N'CurrentUser', @value={user}",
                ct);
        }

        private void GuardAuditTablesAreNotModified()
        {
            IEnumerable<EntityEntry> entityEntries = ChangeTracker.Entries()
                .Where(e =>
                    (e.Entity is ItemAudit || e.Entity is RarityAudit) &&
                    e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);
         
            if (entityEntries.Any()) throw new InvalidOperationException("Audit tables are read-only");
        }
    }
}
