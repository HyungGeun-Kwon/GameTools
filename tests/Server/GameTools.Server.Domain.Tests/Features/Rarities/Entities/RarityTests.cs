using FluentAssertions;
using GameTools.Server.Domain.Features.Rarities.Entities;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using static GameTools.Server.Domain.Tests.TestDatas.Rarities.RarityDomainTestData;

namespace GameTools.Server.Domain.Tests.Features.Rarities.Entities
{
    public class RarityTests
    {
        private static Rarity CreateRarity(
            string? grade = null,
            string? colorCode = null)
            => new(
                RarityId.New(),
                new RarityGrade(grade ?? ValidGrade()),
                new RarityColorCode(colorCode ?? ValidColorCode()));

        [Fact]
        public void ChangeGrade_Should_Change_When_Different()
        {
            var rarity = CreateRarity(ValidGrade('a'));
            var newGrade = new RarityGrade(ValidGrade('b'));

            rarity.ChangeGrade(newGrade);

            rarity.Grade.Should().Be(newGrade);
        }

        [Fact]
        public void ChangeGrade_Should_Throw_When_Null()
        {
            var rarity = CreateRarity();
            RarityGrade? newGrade = null;

            var act = () => rarity.ChangeGrade(newGrade!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ChangeColor_Should_Change_When_Different()
        {
            var rarity = CreateRarity(ValidColorCode("#AAAAAA"));
            var newColor = new RarityColorCode("#FFFFFF");

            rarity.ChangeColor(newColor);

            rarity.ColorCode.Should().Be(newColor);
        }


        [Fact]
        public void ChangeColor_Should_Throw_When_Null()
        {
            var rarity = CreateRarity();

            Action act = () => rarity.ChangeColor(null!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_Should_Throw_When_Arguments_Are_Null()
        {
            var validGrade = new RarityGrade(ValidGrade());
            var validColorCode = new RarityColorCode(ValidColorCode());

            Action act1 = () => _ = new Rarity(null!, validGrade, validColorCode);
            Action act2 = () => _ = new Rarity(RarityId.New(), null!, validColorCode);
            Action act3 = () => _ = new Rarity(RarityId.New(), validGrade, null!);

            act1.Should().Throw<ArgumentNullException>();
            act2.Should().Throw<ArgumentNullException>();
            act3.Should().Throw<ArgumentNullException>();
        }
    }
}
