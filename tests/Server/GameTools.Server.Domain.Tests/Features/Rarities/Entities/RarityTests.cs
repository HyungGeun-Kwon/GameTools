using FluentAssertions;
using GameTools.Server.Domain.Features.Rarities.Entities;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Tests.Features.Rarities.Entities
{
    public class RarityTests
    {
        private static Rarity CreateRarity(
            string grade = "Common",
            string colorCode = "#FFFFFF")
            => new(
                RarityId.New(),
                new RarityGrade(grade),
                new RarityColorCode(colorCode));

        [Fact]
        public void ChangeGrade_Should_Change_When_Different()
        {
            var rarity = CreateRarity(grade: "Common");
            var newGrade = new RarityGrade("Uncommon");
            
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
            var rarity = CreateRarity(colorCode: "#FFFFFF");
            var newColor = new RarityColorCode("#FF0000");

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
            Action act1 = () => _ = new Rarity(null!,          new RarityGrade("Common"), new RarityColorCode("#FFFFFF"));
            Action act2 = () => _ = new Rarity(RarityId.New(), null!,                     new RarityColorCode("#FFFFFF"));
            Action act3 = () => _ = new Rarity(RarityId.New(), new RarityGrade("Common"), null!);

            act1.Should().Throw<ArgumentNullException>();
            act2.Should().Throw<ArgumentNullException>();
            act3.Should().Throw<ArgumentNullException>();
        }
    }
}
