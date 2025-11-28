using FluentAssertions;
using GameTools.Server.Domain.Features.Rarities.Exceptions;
using GameTools.Server.Domain.Features.Rarities.ValueObjects;
using static GameTools.Server.TestUtilities.Domain.Rarities.DomainRarityTestData;

namespace GameTools.Server.Domain.Tests.Features.Rarities.ValueObjects
{
    public class RarityGradeTests
    {
        [Fact]
        public void Ctor_Should_TrimValue()
        {
            var core = ValidRarityGradeValue();
            var raw = $"     {core}     ";

            var name = new RarityGrade(raw);

            name.Value.Should().Be(core);
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
            var a = ValidRarityGrade();
            var b = ValidRarityGrade();

            a.Should().Be(b);
            (a == b).Should().BeTrue();
        }

        [Fact]
        public void ToString_Should_Return_Value_ToString()
        {
            var text = ValidRarityGradeValue();
            var d = new RarityGrade(text);

            var result = d.ToString();

            result.Should().Be(text);
        }
    }
}
