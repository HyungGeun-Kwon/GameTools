using System.Data;
using System.Data.Common;
using GameTools.Server.Application.Abstractions.Users;
using GameTools.Server.Domain.Catalog.Items.Entities;
using GameTools.Server.Domain.Catalog.Rarities.Entities;
using GameTools.Server.Infrastructure.Persistence.Auditing.Models;
using GameTools.Server.Infrastructure.Persistence.Operations.Restores.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GameTools.Server.Infrastructure.Persistence
{
    public sealed class AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser actor) : DbContext(options)
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

            var conn = Database.GetDbConnection();
            var openedHere = false;

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
                openedHere = true;
            }

            try
            {
                SetActor();
                return base.SaveChanges();
            }
            finally
            {
                try { ClearActor(); } catch { }

                if (openedHere)
                    conn.Close();
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            GuardAuditTablesAreNotModified();

            if (!Database.IsRelational())
                return await base.SaveChangesAsync(ct);

            var conn = Database.GetDbConnection();
            var openedHere = false;

            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync(ct);
                openedHere = true;
            }

            try
            {
                await SetActorAsync(ct);
                return await base.SaveChangesAsync(ct);
            }
            finally
            {
                try { await ClearActorAsync(ct); } catch { }

                if (openedHere)
                    await conn.CloseAsync();
            }
        }

        public async Task<T> WithSessionActorAsync<T>(
            Func<DbConnection, Task<T>> work,
            CancellationToken ct = default)
        {
            if (!Database.IsRelational())
                throw new NotSupportedException("WithSessionActorAsync is only supported for relational databases.");

            var conn = Database.GetDbConnection();
            var openedHere = false;

            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync(ct);
                openedHere = true;
            }

            try
            {
                await SetActorAsync(ct);
                return await work(conn);
            }
            finally
            {
                try { await ClearActorAsync(ct); } catch { }

                if (openedHere)
                    await conn.CloseAsync();
            }
        }

        private void SetActor()
        {
            var user = actor.UserIdOrName ?? "unknown";
            Database.ExecuteSqlInterpolated(
                $"EXEC sys.sp_set_session_context @key=N'actor', @value={user}");
        }

        private Task SetActorAsync(CancellationToken ct)
        {
            var user = actor.UserIdOrName ?? "unknown";
            return Database.ExecuteSqlInterpolatedAsync(
                $"EXEC sys.sp_set_session_context @key=N'actor', @value={user}", ct);
        }

        private void ClearActor()
        {
            Database.ExecuteSqlRaw(
                "EXEC sys.sp_set_session_context @key=N'actor', @value=NULL;");
        }

        private Task ClearActorAsync(CancellationToken ct)
        {
            return Database.ExecuteSqlRawAsync(
                "EXEC sys.sp_set_session_context @key=N'actor', @value=NULL;", ct);
        }

        private void GuardAuditTablesAreNotModified()
        {
            IEnumerable<EntityEntry> entityEntries = ChangeTracker.Entries()
                .Where(e =>
                    (e.Entity is ItemAudit || e.Entity is RarityAudit) &&
                    e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

            if (entityEntries.Any())
                throw new InvalidOperationException("Audit tables are read-only");
        }
    }
}
