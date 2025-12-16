using FluentAssertions;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using static GameTools.Server.TestUtilities.Domain.Rarities.DomainRarityTestData;

namespace GameTools.Server.Domain.Tests.Catalog.Rarities.ValueObjects
{
    public class RarityColorCodeTests
    {
        [Fact]
        public void Ctor_Should_Trim_And_Uppercase_Value()
        {
            var core = ValidRarityColorCodeValue();
            var raw = $"   {core}    ";

            var color = new RarityColorCode(raw);

            color.Value.Should().Be(core);
        }

        [Fact]
        public void Ctor_Should_Throw_WhenNull()
        {
            string? value = null;

            var act = () => new RarityColorCode(value!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("#FF00A")]   // too short
        [InlineData("#FF00AAF")] // too long
        [InlineData("FF00AA")]   // missing '#'
        [InlineData("#GG00AA")]  // invalid hex chars
        public void Ctor_Should_Throw_When_Format_Is_Invalid(string raw)
        {
            Action act = () => _ = new RarityColorCode(raw);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Value_ObjectEquality_Should_Work()
        {
            var a = new RarityColorCode(ValidRarityColorCodeValue().ToUpper());
            var b = new RarityColorCode(ValidRarityColorCodeValue().ToLower());

            a.Should().Be(b);
            (a == b).Should().BeTrue();
        }


        [Fact]
        public void ToString_Should_Return_Value()
        {
            var text = ValidRarityColorCodeValue();
            var color = new RarityColorCode(text);

            var result = color.ToString();

            result.Should().Be(text);
        }
    }
}
