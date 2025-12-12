using GameTools.Server.Infrastructure.Persistence.Auditing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameTools.Server.Infrastructure.Persistence.Auditing.Configurations
{
    public sealed class ItemAuditConfiguration : IEntityTypeConfiguration<ItemAudit>
    {
        public void Configure(EntityTypeBuilder<ItemAudit> b)
        {
            b.ToTable(nameof(ItemAudit), t =>
            {
                t.HasCheckConstraint("CK_ItemAudit_Action", "UPPER([Action]) IN ('INSERT','UPDATE','DELETE')");
                t.HasCheckConstraint("CK_ItemAudit_Before_IsJson", "([BeforeJson] IS NULL OR ISJSON([BeforeJson]) = 1)");
                t.HasCheckConstraint("CK_ItemAudit_After_IsJson", "([AfterJson]  IS NULL OR ISJSON([AfterJson])  = 1)");
            });

            b.HasKey(i => i.AuditId);

            b.Property(i => i.AuditId)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            b.Property(i => i.Action)
                .HasConversion<string>()
                .HasMaxLength(10)
                .IsRequired();

            b.Property(i => i.ItemId).IsRequired();

            b.Property(i => i.BeforeJson);

            b.Property(i => i.AfterJson);

            b.Property(i => i.ChangedBy)
                .IsRequired()
                .HasMaxLength(64)
                .HasDefaultValue("unknown");

            b.Property(i => i.ChangedAtUtc)
                .IsRequired()
                .HasPrecision(7)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.HasIndex(i => new { i.ItemId, i.ChangedAtUtc });
            b.HasIndex(i => i.ChangedAtUtc);
        }
    }
}

