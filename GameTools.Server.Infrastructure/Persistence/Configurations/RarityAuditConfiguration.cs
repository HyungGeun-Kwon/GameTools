using GameTools.Server.Domain.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameTools.Server.Infrastructure.Persistence.Configurations
{
    public sealed class RarityAuditConfiguration : IEntityTypeConfiguration<RarityAudit>
    {
        public void Configure(EntityTypeBuilder<RarityAudit> b)
        {
            b.ToTable(nameof(RarityAudit), t =>
            {
                t.HasCheckConstraint("CK_RarityAudit_Action", "UPPER([Action]) IN ('INSERT','UPDATE','DELETE')");
                t.HasCheckConstraint("CK_RarityAudit_Before_IsJson", "([BeforeJson] IS NULL OR ISJSON([BeforeJson]) = 1)");
                t.HasCheckConstraint("CK_RarityAudit_After_IsJson", "([AfterJson]  IS NULL OR ISJSON([AfterJson])  = 1)");
            });

            b.HasKey(r => r.AuditId);
            b.Property(r => r.AuditId)
             .UseIdentityColumn();

            b.Property(r => r.Action)
             .HasConversion<string>()
             .HasMaxLength(10)
             .IsRequired();

            b.Property(r => r.RarityId)
                .IsRequired();

            b.Property(r => r.BeforeJson);

            b.Property(r => r.AfterJson);

            b.Property(r => r.ChangedBy)
             .IsRequired()
             .HasMaxLength(64)
             .HasDefaultValue("system");

            b.Property(r => r.ChangedAtUtc)
                .IsRequired()
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.HasIndex(r => new { r.RarityId, r.ChangedAtUtc });
            b.HasIndex(r => new { r.ChangedAtUtc });
        }
    }
}
