using FluentAssertions;
using GameTools.Server.Domain.Features.Rarities.Exceptions;
using GameTools.Server.Domain.Features.Rarities.Factories;
using GameTools.Server.Domain.Features.Rarities.Policies;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using Moq;

namespace GameTools.Server.Domain.Tests.Features.Rarities.Factories
{
    public class RarityFactoryTests
    {
        private static Mock<IRarityGradeUniquenessPolicy> CreateGradePolicyMock(RarityGrade rarityGrade, bool shouldThrow)
        {
            var policyMock = new Mock<IRarityGradeUniquenessPolicy>();

            if (shouldThrow)
            {
                policyMock
                    .Setup(x => x.EnsureUniqueAsync(rarityGrade, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new RarityGradeNotUniqueException(rarityGrade));
            }
            else
            {
                policyMock
                    .Setup(x => x.EnsureUniqueAsync(rarityGrade, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
            }

            return policyMock;
        }

        private static Mock<IRarityColorCodeUniquenessPolicy> CreateColorCodePolicyMock(RarityColorCode rarityColorCode, bool shouldThrow)
        {
            var policyMock = new Mock<IRarityColorCodeUniquenessPolicy>();

            if (shouldThrow)
            {
                policyMock
                    .Setup(x => x.EnsureUniqueAsync(rarityColorCode, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new RarityColorCodeNotUniqueException(rarityColorCode));
            }
            else
            {
                policyMock
                    .Setup(x => x.EnsureUniqueAsync(rarityColorCode, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
            }

            return policyMock;
        }

        [Fact]
        public async Task CreateAsync_Should_Create_Rarity_When_Grade_And_ColorCode_Are_Unique()
        {
            var grade = new RarityGrade("Common");
            var colorCode = new RarityColorCode("#FFFFFF");

            var gradePolicyMock = CreateGradePolicyMock(grade, false);
            var colorCodePolicyMock = CreateColorCodePolicyMock(colorCode, false);
            var factory = new RarityFactory(gradePolicyMock.Object, colorCodePolicyMock.Object);

            var rarity = await factory.CreateAsync(grade, colorCode, CancellationToken.None);

            rarity.Should().NotBeNull();
            rarity.Grade.Should().Be(grade);
            rarity.ColorCode.Should().Be(colorCode);

            gradePolicyMock.Verify(x => x.EnsureUniqueAsync(grade, It.IsAny<CancellationToken>()), Times.Once);
            colorCodePolicyMock.Verify(x => x.EnsureUniqueAsync(colorCode, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_Rarity_When_Grade_IsNotUnique()
        {
            var grade = new RarityGrade("Common");
            var colorCode = new RarityColorCode("#FFFFFF");

            var gradePolicyMock = CreateGradePolicyMock(grade, true);
            var colorCodePolicyMock = CreateColorCodePolicyMock(colorCode, false);
            var factory = new RarityFactory(gradePolicyMock.Object, colorCodePolicyMock.Object);

            Func<Task> act = async () 
                => await factory.CreateAsync(grade, colorCode, CancellationToken.None);

            await act.Should().ThrowAsync<RarityGradeNotUniqueException>();

            gradePolicyMock.Verify(x => x.EnsureUniqueAsync(grade, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_Rarity_When_ColorCode_IsNotUnique()
        {
            var grade = new RarityGrade("Common");
            var colorCode = new RarityColorCode("#FFFFFF");

            var gradePolicyMock = CreateGradePolicyMock(grade, false);
            var colorCodePolicyMock = CreateColorCodePolicyMock(colorCode, true);
            var factory = new RarityFactory(gradePolicyMock.Object, colorCodePolicyMock.Object);

            Func<Task> act = async ()
                => await factory.CreateAsync(grade, colorCode, CancellationToken.None);

            await act.Should().ThrowAsync<RarityColorCodeNotUniqueException>();

            colorCodePolicyMock.Verify(x => x.EnsureUniqueAsync(colorCode, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
