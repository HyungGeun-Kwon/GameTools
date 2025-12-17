using FluentAssertions;
using GameTools.Server.Domain.Catalog.Rarities.Exceptions;
using GameTools.Server.Domain.Catalog.Rarities.Factories;
using GameTools.Server.Domain.Catalog.Rarities.Policies;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using Moq;
using static GameTools.Server.TestUtilities.Domain.Rarities.DomainRarityTestData;

namespace GameTools.Server.Domain.Tests.Catalog.Rarities.Factories
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

        [Fact]
        public async Task CreateAsync_Should_Create_Rarity_When_Grade_And_ColorCode_Are_Unique()
        {
            var grade = ValidRarityGrade();
            var colorCode = ValidRarityColorCode();

            var gradePolicyMock = CreateGradePolicyMock(grade, false);
            var factory = new RarityFactory(gradePolicyMock.Object);

            var rarity = await factory.CreateAsync(grade, colorCode, CancellationToken.None);

            rarity.Should().NotBeNull();
            rarity.Grade.Should().Be(grade);
            rarity.ColorCode.Should().Be(colorCode);

            gradePolicyMock.Verify(x => x.EnsureUniqueAsync(grade, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_Rarity_When_Grade_IsNotUnique()
        {
            var grade = ValidRarityGrade();
            var colorCode = ValidRarityColorCode();

            var gradePolicyMock = CreateGradePolicyMock(grade, true);
            var factory = new RarityFactory(gradePolicyMock.Object);

            Func<Task> act = async () 
                => await factory.CreateAsync(grade, colorCode, CancellationToken.None);

            await act.Should().ThrowAsync<RarityGradeNotUniqueException>();

            gradePolicyMock.Verify(x => x.EnsureUniqueAsync(grade, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
