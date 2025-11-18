using FluentAssertions;
using GameTools.Server.Domain.Features.Items.Entities;
using GameTools.Server.Domain.Features.Items.ValueObjects;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Tests.Features.Items.Entities
{
    public class ItemTests
    {
        private static Item CreateItem(
            string name = "Sword",
            int price = 100,
            string? description = "description",
            Guid? rarityGuid = null)
            => new(
                ItemId.New(),
                new ItemName(name),
                new ItemPrice(price),
                new ItemDescription(description),
                RarityId.From(rarityGuid ?? Guid.NewGuid()));

        [Fact]
        public void Rename_Should_Change_Name_When_Different()
        {
            var item = CreateItem(name: "Sword");
            var newName = new ItemName("New Sword");

            item.Rename(newName);

            item.Name.Should().Be(newName);
        }

        [Fact]
        public void Rename_Should_Throw_When_Null()
        {
            var item = CreateItem();
            ItemName? newName = null;

            var act = () => item.Rename(newName!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ChangePrice_Should_Change_Price_When_Different()
        {
            var item = CreateItem(price: 100);
            var newPrice = new ItemPrice(200);

            item.ChangePrice(newPrice);

            item.Price.Should().Be(newPrice);
        }

        [Fact]
        public void ChangePrice_Should_Throw_When_Null()
        {
            var item = CreateItem();
            ItemPrice? newPrice = null;

            var act = () => item.ChangePrice(newPrice!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ChangeDescription_Should_Change_Description_When_Different()
        {
            var item = CreateItem(description: "Old description");
            var newDescription = new ItemDescription("New description");

            item.ChangeDescription(newDescription);

            item.Description.Should().Be(newDescription);
        }

        [Fact]
        public void ChangeDescription_Should_Throw_When_Null()
        {
            var item = CreateItem();
            ItemDescription? newDescription = null;

            var act = () => item.ChangeDescription(newDescription!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ChangeRarity_Should_Change_RarityId_When_Different()
        {
            var item = CreateItem(rarityGuid: Guid.NewGuid());
            var newRarityId = RarityId.New();

            item.ChangeRarity(newRarityId);

            item.RarityId.Should().Be(newRarityId);
        }

        [Fact]
        public void ChangeRarity_Should_Throw_When_Null()
        {
            var item = CreateItem();
            RarityId? newRarityId = null;

            var act = () => item.ChangeRarity(newRarityId!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_Should_Throw_When_Arguments_Are_Null()
        {
            Action act1 = () => _ = new Item(null!,        new ItemName("Sword"), new ItemPrice(100), new ItemDescription("desc"), RarityId.New());
            Action act2 = () => _ = new Item(ItemId.New(), null!,                 new ItemPrice(100), new ItemDescription("desc"), RarityId.New());
            Action act3 = () => _ = new Item(ItemId.New(), new ItemName("Sword"), null!,              new ItemDescription("desc"), RarityId.New());
            Action act4 = () => _ = new Item(ItemId.New(), new ItemName("Sword"), new ItemPrice(100), null!,                       RarityId.New());
            Action act5 = () => _ = new Item(ItemId.New(), new ItemName("Sword"), new ItemPrice(100), new ItemDescription("desc"), null!);

            act1.Should().Throw<ArgumentNullException>();
            act2.Should().Throw<ArgumentNullException>();
            act3.Should().Throw<ArgumentNullException>();
            act4.Should().Throw<ArgumentNullException>();
            act5.Should().Throw<ArgumentNullException>();
        }
    }
}
