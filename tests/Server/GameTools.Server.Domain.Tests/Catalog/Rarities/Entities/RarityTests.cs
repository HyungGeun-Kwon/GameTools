using FluentAssertions;
using GameTools.Server.Domain.Catalog.Rarities.Entities;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using static GameTools.Server.TestUtilities.Domain.Rarities.DomainRarityTestData;

namespace GameTools.Server.Domain.Tests.Catalog.Rarities.Entities
{
    public class RarityTests
    {
        [Fact]
        public void ChangeGrade_Should_Change_When_Different()
        {
            var rarity = BuildRarity(grade: ValidRarityGrade('a'));
            var newGrade = ValidRarityGrade('b');

            rarity.ChangeGrade(newGrade);

            rarity.Grade.Should().Be(newGrade);
        }

        [Fact]
        public void ChangeGrade_Should_Throw_When_Null()
        {
            var rarity = BuildRarity();
            RarityGrade? newGrade = null;

            var act = () => rarity.ChangeGrade(newGrade!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ChangeColor_Should_Change_When_Different()
        {
            var rarity = BuildRarity(colorCode: ValidRarityColorCode("#AAAAAA"));
            var newColor = ValidRarityColorCode("#FFFFFF");

            rarity.ChangeColor(newColor);

            rarity.ColorCode.Should().Be(newColor);
        }


        [Fact]
        public void ChangeColor_Should_Throw_When_Null()
        {
            var rarity = BuildRarity();

            Action act = () => rarity.ChangeColor(null!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_Should_Throw_When_Arguments_Are_Null()
        {
            var validGrade = ValidRarityGrade();
            var validColorCode = ValidRarityColorCode();

            Action act1 = () => _ = new Rarity(null!, validGrade, validColorCode);
            Action act2 = () => _ = new Rarity(RarityId.New(), null!, validColorCode);
            Action act3 = () => _ = new Rarity(RarityId.New(), validGrade, null!);

            act1.Should().Throw<ArgumentNullException>();
            act2.Should().Throw<ArgumentNullException>();
            act3.Should().Throw<ArgumentNullException>();
        }
    }
}
