using FluentAssertions;
using GameTools.Server.Domain.Catalog.Items.Exceptions;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;
using static GameTools.Server.TestUtilities.Domain.Items.DomainItemTestData;

namespace GameTools.Server.Domain.Tests.Catalog.Items.ValueObjects
{
    public class ItemNameTests
    {
        [Fact]
        public void Ctor_Should_TrimValue()
        {
            var core = ValidItemNameValue();
            var raw = $"     {core}     ";

            var name = new ItemName(raw);

            name.Value.Should().Be(core);
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
            var text = ValidItemNameValue();

            var a = new ItemName(text);
            var b = new ItemName(text);

            a.Should().Be(b);
            (a == b).Should().BeTrue();
        }

        [Fact]
        public void ToString_Should_Return_Value_ToString()
        {
            var text = ValidItemNameValue();
            var d = new ItemName(text);

            var result = d.ToString();

            result.Should().Be(text);
        }
    }
}
