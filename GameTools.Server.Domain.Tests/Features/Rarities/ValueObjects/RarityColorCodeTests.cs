using FluentAssertions;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Tests.Features.Rarities.ValueObjects
{
    public class RarityColorCodeTests
    {
        [Fact]
        public void Ctor_Should_Trim_And_Uppercase_Value()
        {
            var raw = "   #ff00aa   ";

            var color = new RarityColorCode(raw);

            color.Value.Should().Be("#FF00AA");
        }

        [Fact]
        public void Ctor_Should_Throw_WhenNull()
        {
            string? value = null;

            var act = () => new RarityColorCode(value!);

            act.Should()
               .Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("value");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("#FF00A")]   // too short
        [InlineData("#FF00AAF")] // too long
        [InlineData("FF00AA")]   // missing '#'
        [InlineData("#GG00AA")]  // invalid hex chars
        public void Ctor_Should_Throw_When_Format_Is_Invalid(string raw)
        {
            Action act = () => _ = new RarityColorCode(raw);

            act.Should()
               .Throw<ArgumentException>()
               .And.ParamName.Should().Be("value");
        }

        [Fact]
        public void Value_ObjectEquality_Should_Work()
        {
            var a = new RarityColorCode("#FF00AA");
            var b = new RarityColorCode("#ff00aa");

            a.Should().Be(b);
            (a == b).Should().BeTrue();
        }


        [Fact]
        public void ToString_Should_Return_Value()
        {
            var color = new RarityColorCode("#FF00AA");

            var text = color.ToString();

            text.Should().Be("#FF00AA");
        }
    }
}
