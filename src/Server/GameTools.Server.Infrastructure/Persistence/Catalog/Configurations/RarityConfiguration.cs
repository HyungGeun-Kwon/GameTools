using GameTools.Server.Domain.Features.Rarities.Entities;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameTools.Server.Infrastructure.Persistence.Catalog.Configurations
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
                    "LEN([ColorCode]) = 7 " +
                    "AND LEFT([ColorCode],1) = '#' " +
                    "AND [ColorCode] = UPPER([ColorCode]) " +
                    "AND [ColorCode] LIKE '#[0-9A-F][0-9A-F][0-9A-F][0-9A-F][0-9A-F][0-9A-F]'"
                );
                // 앞뒤 공백 금지
                t.HasCheckConstraint(
                    "CK_Rarity_ColorCode_NoSpaces",
                    "RTRIM(LTRIM([ColorCode])) = [ColorCode]"
                );

                t.HasTrigger("trg_Rarity_Audit");
            });

            b.HasKey(r => r.Id);

            b.Property(r => r.Id)
                .HasConversion(id => id.Value, v => RarityId.From(v))
                .ValueGeneratedNever();

            b.Property(r => r.Grade)
                .HasConversion(grade => grade.Value, v => new RarityGrade(v))
                .HasMaxLength(RarityGrade.MaxLength)
                .IsRequired();

            b.Property(x => x.ColorCode)
                .HasConversion(c => c.Value, v => new RarityColorCode(v))
                .HasMaxLength(RarityColorCode.Length)
                .IsRequired();

            b.Property<byte[]>("RowVersion").IsRowVersion();

            b.HasIndex(r => r.Grade).IsUnique();
        }
    }
}