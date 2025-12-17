using FluentAssertions;
using GameTools.Server.Domain.Catalog.Items.Entities;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using static GameTools.Server.TestUtilities.Domain.Items.DomainItemTestData;

namespace GameTools.Server.Domain.Tests.Catalog.Items.Entities
{
    public class ItemTests
    {
        [Fact]
        public void Rename_Should_Change_When_Different()
        {
            var item = BuildItem(name: ValidItemName('a'));
            var newName = ValidItemName('b');

            item.Rename(newName);

            item.Name.Should().Be(newName);
        }

        [Fact]
        public void Rename_Should_Throw_When_Null()
        {
            var item = BuildItem();
            ItemName? newName = null;

            var act = () => item.Rename(newName!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ChangePrice_Should_Change_When_Different()
        {
            var item = BuildItem(price: ValidItemPrice(ItemPrice.MinValue));
            var newPrice = new ItemPrice(ItemPrice.MinValue + 1);

            item.ChangePrice(newPrice);

            item.Price.Should().Be(newPrice);
        }

        [Fact]
        public void ChangePrice_Should_Throw_When_Null()
        {
            var item = BuildItem();
            ItemPrice? newPrice = null;

            var act = () => item.ChangePrice(newPrice!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ChangeDescription_Should_Change_When_Different()
        {
            var item = BuildItem(description: ValidItemDescription('a'));
            var newDescription = ValidItemDescription('b');

            item.ChangeDescription(newDescription);

            item.Description.Should().Be(newDescription);
        }

        [Fact]
        public void ChangeDescription_Should_Throw_When_Null()
        {
            var item = BuildItem();
            ItemDescription? newDescription = null;

            var act = () => item.ChangeDescription(newDescription!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ChangeRarity_Should_Change_When_Different()
        {
            var item = BuildItem(rarityId: RarityId.New());
            var newRarityId = RarityId.New();

            item.ChangeRarity(newRarityId);

            item.RarityId.Should().Be(newRarityId);
        }

        [Fact]
        public void ChangeRarity_Should_Throw_When_Null()
        {
            var item = BuildItem();
            RarityId? newRarityId = null;

            var act = () => item.ChangeRarity(newRarityId!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_Should_Throw_When_Arguments_Are_Null()
        {
            var validName = ValidItemName();
            var validPrice = ValidItemPrice();
            var validDescription = ValidItemDescription();
            var validRarity = RarityId.New();

            Action act1 = () => _ = new Item(null!, validName, validPrice, validDescription, validRarity);
            Action act2 = () => _ = new Item(ItemId.New(), null!, validPrice, validDescription, validRarity);
            Action act3 = () => _ = new Item(ItemId.New(), validName, null!, validDescription, validRarity);
            Action act4 = () => _ = new Item(ItemId.New(), validName, validPrice, null!, validRarity);
            Action act5 = () => _ = new Item(ItemId.New(), validName, validPrice, validDescription, null!);

            act1.Should().Throw<ArgumentNullException>();
            act2.Should().Throw<ArgumentNullException>();
            act3.Should().Throw<ArgumentNullException>();
            act4.Should().Throw<ArgumentNullException>();
            act5.Should().Throw<ArgumentNullException>();
        }
    }
}
