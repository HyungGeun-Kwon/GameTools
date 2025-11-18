using FluentAssertions;
using GameTools.Server.Domain.Features.Items.Exceptions;
using GameTools.Server.Domain.Features.Items.Policies;
using GameTools.Server.Domain.Features.Items.Services;
using GameTools.Server.Domain.Features.Items.ValueObjects;
using Moq;

namespace GameTools.Server.Domain.Tests.Features.Items.Policies
{
    public class ItemNameUniquenessPolicyTests
    {
        private static Mock<IItemNameUniquenessChecker> CreateNameCheckerMock(bool reValue)
        {
            var checkerMock = new Mock<IItemNameUniquenessChecker>();
            checkerMock
                .Setup(x => x.ExistsAsync(new ItemName("test"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(reValue);

            return checkerMock;
        }

        [Fact]
        public async Task EnsureUniqueAsync_Should_NotThrow_When_Name_IsUnique()
        {
            var checkerMock = CreateNameCheckerMock(false);

            var policy = new ItemNameUniquenessPolicy(checkerMock.Object);
            var name = new ItemName("Sword");
            
            Func<Task> act = async () => await policy.EnsureUniqueAsync(name, CancellationToken.None);
            
            await act.Should().NotThrowAsync();
            checkerMock.Verify(x => x.ExistsAsync(name, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EnsureUniqueAsync_Should_Throw_When_Name_IsNotUnique()
        {
            var checkerMock = CreateNameCheckerMock(true);

            var policy = new ItemNameUniquenessPolicy(checkerMock.Object);
            var name = new ItemName("Sword");
            
            Func<Task> act = async () => await policy.EnsureUniqueAsync(name, CancellationToken.None);
            
            await act.Should().ThrowAsync<ItemNameNotUniqueException>();
            checkerMock.Verify(x => x.ExistsAsync(name, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
