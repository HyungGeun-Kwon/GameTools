using GameTools.Server.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameTools.Server.Infrastructure.Persistence.Configurations
{
    public sealed class RestoreRunConfiguration : IEntityTypeConfiguration<RestoreRunRow>
    {
        public void Configure(EntityTypeBuilder<RestoreRunRow> b)
        {
            b.ToTable("RestoreRun");

            b.HasKey(x => x.RestoreId);

            b.Property(x => x.Actor).IsRequired().HasMaxLength(128);
            b.Property(x => x.DryRun).IsRequired();

            b.Property(x => x.StartedAtUtc)
                .IsRequired()
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.Property(x => x.EndedAtUtc);

            b.Property(x => x.AsOfUtc).IsRequired();

            b.Property(x => x.AffectedCounts);
            b.Property(x => x.Notes);
            b.Property(x => x.FiltersJson);

            b.HasIndex(x => x.StartedAtUtc);
            b.HasIndex(x => new { x.Actor, x.StartedAtUtc });
            b.HasIndex(x => x.DryRun);
        }
    }
}
