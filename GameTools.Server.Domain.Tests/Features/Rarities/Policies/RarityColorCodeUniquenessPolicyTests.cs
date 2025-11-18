using FluentAssertions;
using GameTools.Server.Domain.Features.Rarities.Exceptions;
using GameTools.Server.Domain.Features.Rarities.Policies;
using GameTools.Server.Domain.Features.Rarities.Services;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using Moq;

namespace GameTools.Server.Domain.Tests.Features.Rarities.Policies
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
            var color = new RarityColorCode("#FFFFFF");

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
            var color = new RarityColorCode("#FFFFFF");

            Func<Task> act = async () =>
                await policy.EnsureUniqueAsync(color, CancellationToken.None);

            await act.Should().ThrowAsync<RarityColorCodeNotUniqueException>();
            checkerMock.Verify(x => x.ExistsAsync(color, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
