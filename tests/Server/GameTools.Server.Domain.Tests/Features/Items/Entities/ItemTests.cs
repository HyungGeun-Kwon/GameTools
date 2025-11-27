using FluentAssertions;
using GameTools.Server.Domain.Features.Items.Entities;
using GameTools.Server.Domain.Features.Items.ValueObjects;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using static GameTools.Server.Domain.Tests.TestDatas.Items.ItemDomainTestData;

namespace GameTools.Server.Domain.Tests.Features.Items.Entities
{
    public class ItemTests
    {
        private static Item CreateItem(
            string? name = null,
            int? price = null,
            string? description = null,
            Guid? rarityGuid = null)
            => new(
                ItemId.New(),
                new ItemName(name ?? ValidName()),
                new ItemPrice(ValidPrice(price)),
                new ItemDescription(description ?? ValidDescription()),
                RarityId.From(rarityGuid ?? Guid.NewGuid()));


        [Fact]
        public void Rename_Should_Change_When_Different()
        {
            var item = CreateItem(name: ValidName('a'));
            var newName = new ItemName(ValidName('b'));

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
        public void ChangePrice_Should_Change_When_Different()
        {
            var item = CreateItem(price: ItemPrice.MinValue);
            var newPrice = new ItemPrice(ItemPrice.MinValue + 1);

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
        public void ChangeDescription_Should_Change_When_Different()
        {
            var item = CreateItem(description: ValidDescription('a'));
            var newDescription = new ItemDescription(ValidDescription('b'));

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
        public void ChangeRarity_Should_Change_When_Different()
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
            var validName = new ItemName(ValidName());
            var validPrice = new ItemPrice(ValidPrice());
            var validDescription = new ItemDescription(ValidDescription());
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
