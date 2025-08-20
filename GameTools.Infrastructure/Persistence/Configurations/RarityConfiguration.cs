using GameTools.Domain.Common.Rules;
using GameTools.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameTools.Infrastructure.Persistence.Configurations
{
    public sealed class RarityConfiguration : IEntityTypeConfiguration<Rarity>
    {
        public void Configure(EntityTypeBuilder<Rarity> b)
        {
            b.ToTable(nameof(Rarity), t =>
            {
                // 제약 #RRGGBB(대문자)
                t.HasCheckConstraint(
                "CK_Rarity_ColorCode_Format",
                "LEN([ColorCode]) = 7 AND LEFT([ColorCode],1) = '#' " +
                "AND [ColorCode] = UPPER([ColorCode]) " +
                "AND [ColorCode] LIKE '#[0-9A-F][0-9A-F][0-9A-F][0-9A-F][0-9A-F][0-9A-F]'");
                t.HasTrigger("trg_Rarity_Audit");
            });

            b.HasKey(r => r.Id);
            b.Property(r => r.Id)
             .UseIdentityColumn();

            b.Property(i => i.RowVersion)
                .IsRequired()
                .IsRowVersion();

            b.Property(r => r.Grade)
             .IsRequired()
             .HasMaxLength(RarityRules.GradeMax);

            b.Property(r => r.ColorCode)
             .IsRequired()
             .HasColumnType("char(7)") // '#RRGGBB'
             .HasDefaultValue("#A0A0A0");

            b.HasIndex(r => r.Grade)
             .IsUnique();

            b.HasIndex(r => r.ColorCode)
             .IsUnique();

            // 컬렉션 백킹필드(_items) 사용
            b.Navigation(r => r.Items)
             .HasField("_items")
             .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}