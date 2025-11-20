using FluentAssertions;
using GameTools.Server.Domain.Features.Rarities.Exceptions;
using GameTools.Server.Domain.Features.Rarities.Policies;
using GameTools.Server.Domain.Features.Rarities.Services;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using Moq;

namespace GameTools.Server.Domain.Tests.Features.Rarities.Policies
{
    public class RarityGradeUniquenessPolicyTests
    {
        private static Mock<IRarityGradeUniquenessChecker> CreateGradeCheckerMock(bool exists)
        {
            var checkerMock = new Mock<IRarityGradeUniquenessChecker>();
            checkerMock
                .Setup(x => x.ExistsAsync(It.IsAny<RarityGrade>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(exists);
            return checkerMock;
        }

        [Fact]
        public async Task EnsureUniqueAsync_Should_NotThrow_When_Grade_IsUnique()
        {
            var checkerMock = CreateGradeCheckerMock(false);

            var policy = new RarityGradeUniquenessPolicy(checkerMock.Object);
            var grade = new RarityGrade("Common");

            Func<Task> act = async () => await policy.EnsureUniqueAsync(grade, CancellationToken.None);

            await act.Should().NotThrowAsync();
            checkerMock.Verify(x => x.ExistsAsync(grade, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EnsureUniqueAsync_Should_Throw_When_Grade_IsNotUnique()
        {
            var checkerMock = CreateGradeCheckerMock(true);
            var policy = new RarityGradeUniquenessPolicy(checkerMock.Object);
            var grade = new RarityGrade("Common");

            Func<Task> act = async () => await policy.EnsureUniqueAsync(grade, CancellationToken.None);

            await act.Should().ThrowAsync<RarityGradeNotUniqueException>();
            checkerMock.Verify( x => x.ExistsAsync(grade, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
