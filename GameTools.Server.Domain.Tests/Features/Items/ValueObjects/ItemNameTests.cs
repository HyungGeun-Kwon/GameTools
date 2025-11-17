using FluentAssertions;
using GameTools.Server.Domain.Features.Items.Exceptions;
using GameTools.Server.Domain.Features.Items.ValueObjects;

namespace GameTools.Server.Domain.Tests.Features.Items.ValueObjects
{
    public class ItemNameTests
    {
        [Fact]
        public void Ctor_Should_TrimValue()
        {
            var raw = "     Sword     ";
            var name = new ItemName(raw);

            name.Value.Should().Be("Sword");
        }

        [Fact]
        public void Ctor_Should_Throw_When_ValueIsNull()
        {
            string? raw = null;

            Action act = () => _ = new ItemName(raw!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        public void Ctor_Should_Throw_When_Whitespace(string raw)
        {
            Action act = () => _ = new ItemName(raw);

            act.Should().Throw<ItemNameTooShortException>();
        }

        [Fact]
        public void Ctor_Should_Throw_WhenTooShort()
        {
            var tooShort = new string('a', ItemName.MinLength - 1);
            Action act = () => _ = new ItemName(tooShort);

            act.Should().Throw<ItemNameTooShortException>();
        }

        [Fact]
        public void Ctor_Should_Throw_WhenTooShort_AfterTrim()
        {
            var raw = " " + new string('a', ItemName.MinLength - 1) + " ";
            Action act = () => _ = new ItemName(raw);

            act.Should().Throw<ItemNameTooShortException>();
        }

        [Fact]
        public void Ctor_Should_Throw_WhenTooLong()
        {
            var tooLong = new string('a', ItemName.MaxLength + 1);
            Action act = () => _ = new ItemName(tooLong);
         
            act.Should().Throw<ItemNameTooLongException>();
        }

        [Fact]
        public void Ctor_Should_Allow_MinAndMaxLength()
        {
            var min = new string('a', ItemName.MinLength);
            var max = new string('a', ItemName.MaxLength);

            var minName = new ItemName(min);
            var maxName = new ItemName(max);

            minName.Value.Should().Be(min);
            maxName.Value.Should().Be(max);
        }

        [Fact]
        public void Value_Equality_Should_Work()
        {
            var a = new ItemName("Sword");
            var b = new ItemName("Sword");

            a.Should().Be(b);
            (a == b).Should().BeTrue();
        }
        [Fact]
        public void ToString_Should_Return_Value_ToString()
        {
            var d = new ItemName("Sword");

            var text = d.ToString();

            text.Should().Be("Sword");
        }
    }
}
