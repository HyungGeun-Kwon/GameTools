using GameTools.Server.Infrastructure.Persistence.Operations.Restores.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameTools.Server.Infrastructure.Persistence.Operations.Restores.Configurations
{
    public sealed class RestoreHistoryConfiguration : IEntityTypeConfiguration<RestoreHistory>
    {
        public void Configure(EntityTypeBuilder<RestoreHistory> b)
        {
            b.ToTable(nameof(RestoreHistory));

            b.HasKey(x => x.RestoreId);

            b.Property(x => x.Actor)
                .IsRequired()
                .HasMaxLength(128)
                .HasDefaultValue("unknown");

            b.Property(x => x.DryRun).IsRequired();

            b.Property(x => x.StartedAtUtc)
                .IsRequired()
                .HasPrecision(7)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.Property(x => x.EndedAtUtc).HasPrecision(7);

            b.Property(x => x.AsOfUtc).IsRequired().HasPrecision(7);

            b.Property(x => x.AffectedCounts).HasColumnType("nvarchar(max)");
            b.Property(x => x.Notes).HasColumnType("nvarchar(max)");
            b.Property(x => x.FiltersJson).HasColumnType("nvarchar(max)");

            b.HasIndex(x => x.StartedAtUtc);
            b.HasIndex(x => new { x.Actor, x.StartedAtUtc });
            b.HasIndex(x => x.DryRun);
        }
    }
}
