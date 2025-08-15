using GameTools.Domain.Common.Rules;
using GameTools.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameTools.Infrastructure.Persistence.Configurations
{
    public sealed class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> b)
        {
            b.ToTable(nameof(Item), t =>
            {
                // 제약 Price >= 0
                t.HasCheckConstraint("CK_Item_Price_NonNegative", "[Price] >= 0");
            });

            b.HasKey(i => i.Id);
            b.Property(i => i.Id).UseIdentityColumn(); // 자동증가

            // Columns
            b.Property(i => i.Name)
             .IsRequired()
             .HasMaxLength(ItemRules.NameMax);

            b.Property(i => i.Price)
             .IsRequired();

            b.Property(i => i.Description)
             .HasMaxLength(ItemRules.DescriptionMax);

            b.Property( i => i.RarityId)
             .IsRequired();

            // FK
            b.HasOne(i => i.Rarity)
             .WithMany(r => r.Items)
             .HasForeignKey(i => i.RarityId)
             .OnDelete(DeleteBehavior.Restrict);

            // Index
            b.HasIndex(i => i.RarityId);

            b.HasIndex(i => i.Name).IsUnique();
        }
    }
}
