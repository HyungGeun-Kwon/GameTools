using FluentAssertions;
using GameTools.Server.Domain.Catalog.Rarities.Exceptions;
using GameTools.Server.Domain.Catalog.Rarities.Policies;
using GameTools.Server.Domain.Catalog.Rarities.Services;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using Moq;
using static GameTools.Server.TestUtilities.Domain.Rarities.DomainRarityTestData;

namespace GameTools.Server.Domain.Tests.Catalog.Rarities.Policies
{
    public class RarityColorCodeUniquenessPolicyTests
    {
        private static Mock<IRarityColorCodeUniquenessChecker> CreateColorCheckerMock(bool exists)
        {
            var checkerMock = new Mock<IRarityColorCodeUniquenessChecker>();
            checkerMock
                .Setup(x => x.ExistsAsync(It.IsAny<RarityColorCode>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(exists);
            return checkerMock;
        }

        [Fact]
        public async Task EnsureUniqueAsync_Should_NotThrow_When_Color_IsUnique()
        {
            var checkerMock = CreateColorCheckerMock(false);

            var policy = new RarityColorCodeUniquenessPolicy(checkerMock.Object);
            var color = ValidRarityColorCode();

            Func<Task> act = async () => await policy.EnsureUniqueAsync(color, CancellationToken.None);

            await act.Should().NotThrowAsync();
            checkerMock.Verify(
                x => x.ExistsAsync(color, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task EnsureUniqueAsync_Should_Throw_When_Color_IsNotUnique()
        {
            var checkerMock = CreateColorCheckerMock(exists: true);

            var policy = new RarityColorCodeUniquenessPolicy(checkerMock.Object);
            var color = ValidRarityColorCode();

            Func<Task> act = async () =>
                await policy.EnsureUniqueAsync(color, CancellationToken.None);

            await act.Should().ThrowAsync<RarityColorCodeNotUniqueException>();
            checkerMock.Verify(x => x.ExistsAsync(color, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
