using GameTools.Server.Domain.Features.Items.Entities;
using GameTools.Server.Domain.Features.Items.ValueObjects;
using GameTools.Server.Domain.Features.Rarities.Entities;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameTools.Server.Infrastructure.Persistence.Catalog.Configurations
{
    public sealed class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> b)
        {
            b.ToTable(nameof(Item), t =>
            {
                t.HasCheckConstraint("CK_Item_Price_NonNegative", "[Price] >= 0");
                t.HasTrigger("trg_Item_Audit");
            });

            b.HasKey(i => i.Id);

            b.Property(i => i.Id)
                .HasConversion(id => id.Value, v => ItemId.From(v))
                .ValueGeneratedNever();

            b.Property(i => i.RarityId)
                .HasConversion(rId => rId.Value, v => RarityId.From(v))
                .IsRequired();


            b.HasOne<Rarity>()
                .WithMany()
                .HasForeignKey(x => x.RarityId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Items_Rarities_RarityId");

            b.Property(i => i.Name)
                .HasConversion(name => name.Value, v => new ItemName(v))
                .HasMaxLength(ItemName.MaxLength)
                .IsRequired();

            b.Property(i => i.Description)
                .HasConversion(description => description.Value, v => new ItemDescription(v))
                .HasMaxLength(ItemDescription.MaxLength)
                .IsRequired(false);

            b.Property(i => i.Price)
                .HasConversion(price => price.Value, v => new ItemPrice(v))
                .IsRequired();

            b.Property<byte[]>("RowVersion").IsRequired().IsRowVersion();

            b.HasIndex(i => i.Name).IsUnique();
            b.HasIndex(i => i.RarityId);

        }
    }
}
