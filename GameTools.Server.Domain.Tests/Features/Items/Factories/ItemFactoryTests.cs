using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GameTools.Server.Domain.Features.Items.Exceptions;
using GameTools.Server.Domain.Features.Items.Factories;
using GameTools.Server.Domain.Features.Items.Policies;
using GameTools.Server.Domain.Features.Items.Services;
using GameTools.Server.Domain.Features.Items.ValueObjects;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using Moq;

namespace GameTools.Server.Domain.Tests.Features.Items.Factories
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
            var name = new ItemName("Sword");
            var price = new ItemPrice(100);
            var description = new ItemDescription("description");
            var rarityId = RarityId.New();

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
            var name = new ItemName("Sword");
            var price = new ItemPrice(100);
            var description = new ItemDescription("description");
            var rarityId = RarityId.New();

            var policyMock = CreateNamePolicyMock(name, true);
            var factory = new ItemFactory(policyMock.Object);

            Func<Task> act = async () 
                => await factory.CreateAsync(name, price, description, rarityId, CancellationToken.None);

            await act.Should().ThrowAsync<ItemNameNotUniqueException>();

            policyMock.Verify(x => x.EnsureUniqueAsync(name, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
