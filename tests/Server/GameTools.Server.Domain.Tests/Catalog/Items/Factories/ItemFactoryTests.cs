using FluentAssertions;
using GameTools.Server.Domain.Catalog.Items.Exceptions;
using GameTools.Server.Domain.Catalog.Items.Factories;
using GameTools.Server.Domain.Catalog.Items.Policies;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;
using Moq;
using static GameTools.Server.TestUtilities.Domain.Items.DomainItemTestData;

namespace GameTools.Server.Domain.Tests.Catalog.Items.Factories
{
    public class ItemFactoryTests
    {
        private static Mock<IItemNameUniquenessPolicy> CreateNamePolicyMock(ItemName itemName, bool shouldThrow)
        {
            var policyMock = new Mock<IItemNameUniquenessPolicy>();

            if (shouldThrow)
            {
                policyMock
                    .Setup(x => x.EnsureUniqueAsync(itemName, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new ItemNameNotUniqueException(itemName));
            }
            else
            {
                policyMock
                    .Setup(x => x.EnsureUniqueAsync(itemName, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
            }

            return policyMock;
        }
        [Fact]
        public async Task CreateAsync_Should_Create_Item_When_Name_IsUnique()
        {
            var name = ValidItemName();
            var price = ValidItemPrice();
            var description = ValidItemDescription();
            var rarityId = ValidRarityId();

            var policyMock = CreateNamePolicyMock(name, false);
            var factory = new ItemFactory(policyMock.Object);

            var item = await factory.CreateAsync(name, price, description, rarityId, CancellationToken.None);

            item.Should().NotBeNull();
            item.Name.Should().Be(name);
            item.Price.Should().Be(price);
            item.Description.Should().Be(description);
            item.RarityId.Should().Be(rarityId);

            policyMock.Verify(x => x.EnsureUniqueAsync(name, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Name_IsNotUnique()
        {
            var name = ValidItemName();
            var price = ValidItemPrice();
            var description = ValidItemDescription();
            var rarityId = ValidRarityId();

            var policyMock = CreateNamePolicyMock(name, true);
            var factory = new ItemFactory(policyMock.Object);

            Func<Task> act = async ()
                => await factory.CreateAsync(name, price, description, rarityId, CancellationToken.None);

            await act.Should().ThrowAsync<ItemNameNotUniqueException>();

            policyMock.Verify(x => x.EnsureUniqueAsync(name, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
