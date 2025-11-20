using FluentAssertions;
using GameTools.Server.Domain.Features.Rarities.Exceptions;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Tests.Features.Rarities.ValueObjects
{
    public class RarityGradeTests
    {
        [Fact]
        public void Ctor_Should_TrimValue()
        {
            var raw = "     Common     ";

            var name = new RarityGrade(raw);

            name.Value.Should().Be("Common");
        }

        [Fact]
        public void Ctor_Should_Throw_When_ValueIsNull()
        {
            string? raw = null;

            Action act = () => _ = new RarityGrade(raw!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        public void Ctor_Should_Throw_When_Whitespace(string raw)
        {
            Action act = () => _ = new RarityGrade(raw);

            act.Should().Throw<RarityGradeTooShortException>();
        }

        [Fact]
        public void Ctor_Should_Throw_WhenTooShort()
        {
            var tooShort = new string('a', RarityGrade.MinLength - 1);
            Action act = () => _ = new RarityGrade(tooShort);

            act.Should().Throw<RarityGradeTooShortException>();
        }

        [Fact]
        public void Ctor_Should_Throw_WhenTooShort_AfterTrim()
        {
            var raw = " " + new string('a', RarityGrade.MinLength - 1) + " ";
            Action act = () => _ = new RarityGrade(raw);

            act.Should().Throw<RarityGradeTooShortException>();
        }

        [Fact]
        public void Ctor_Should_Throw_WhenTooLong()
        {
            var tooLong = new string('a', RarityGrade.MaxLength + 1);
            Action act = () => _ = new RarityGrade(tooLong);

            act.Should().Throw<RarityGradeTooLongException>();
        }

        [Fact]
        public void Ctor_Should_Allow_MinAndMaxLength()
        {
            var min = new string('a', RarityGrade.MinLength);
            var max = new string('a', RarityGrade.MaxLength);

            var minName = new RarityGrade(min);
            var maxName = new RarityGrade(max);

            minName.Value.Should().Be(min);
            maxName.Value.Should().Be(max);
        }

        [Fact]
        public void Value_Equality_Should_Work()
        {
            var a = new RarityGrade("Common");
            var b = new RarityGrade("Common");

            a.Should().Be(b);
            (a == b).Should().BeTrue();
        }

        [Fact]
        public void ToString_Should_Return_Value_ToString()
        {
            var d = new RarityGrade("Sword");

            var text = d.ToString();

            text.Should().Be("Sword");
        }

        [Fact]
        public void Parse_Should_Create_Instance()
        {
            var text = "Common";

            var grade = RarityGrade.Parse(text);

            grade.Value.Should().Be("Common");
        }

        [Fact]
        public void TryParse_Should_ReturnTrue_And_Result_When_Valid()
        {
            var text = "   Common   ";

            var success = RarityGrade.TryParse(text, out var result);

            success.Should().BeTrue();
            result.Should().NotBeNull();
            result!.Value.Should().Be("Common");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void TryParse_Should_ReturnFalse_When_TooShort(string raw)
        {
            var success = RarityGrade.TryParse(raw, out var result);

            success.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void TryParse_Should_ReturnFalse_When_TooLong()
        {
            var raw = new string('a', RarityGrade.MaxLength + 10);

            var success = RarityGrade.TryParse(raw, out var result);

            success.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void TryParse_Should_ReturnFalse_When_Value_Is_Null()
        {
            string? raw = null;

            var success = RarityGrade.TryParse(raw!, out var result);

            success.Should().BeFalse();
            result.Should().BeNull();
        }
    }
}
