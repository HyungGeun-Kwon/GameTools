using FluentAssertions;
using GameTools.Server.Domain.Features.Items.Exceptions;
using GameTools.Server.Domain.Features.Items.ValueObjects;
using static GameTools.Server.Domain.Tests.TestDatas.Items.ItemDomainTestData;

namespace GameTools.Server.Domain.Tests.Features.Items.ValueObjects
{
    public class ItemDescriptionTests
    {
        [Fact]
        public void Ctor_Should_TrimValue()
        {
            var valid = ValidDescription();
            var raw = $"       {valid}       ";

            var name = new ItemDescription(raw);

            name.Value.Should().Be(valid);
        }


        [Fact]
        public void Ctor_Should_Allow_Null()
        {
            string? raw = null;

            var d = new ItemDescription(raw);

            d.Value.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        public void Ctor_Should_Allow_Whitespace_As_NoDescription(string raw)
        {
            var d = new ItemDescription(raw);

            d.Value.Should().BeNull();
        }


        [Fact]
        public void Ctor_Should_Allow_MaxLength()
        {
            var text = new string('a', ItemDescription.MaxLength);

            var d = new ItemDescription(text);

            d.Value.Should().Be(text);
        }

        [Fact]
        public void Ctor_Should_Allow_Value_TooLong_BeforeTrim_But_MaxLength_AfterTrim()
        {
            var text = new string('a', ItemDescription.MaxLength);
            var raw = " " + text + " ";

            var description = new ItemDescription(raw);

            description.Value.Should().Be(text);
        }

        [Fact]
        public void Ctor_Should_Throw_WhenTooLong()
        {
            var tooLong = new string('a', ItemDescription.MaxLength + 1);
            Action act = () => _ = new ItemDescription(tooLong);

            act.Should().Throw<ItemDescriptionTooLongException>();
        }

        [Fact]
        public void Value_Equality_ShouldWork()
        {
            var text = ValidDescription();

            var d1 = new ItemDescription(text);
            var d2 = new ItemDescription(text);

            d1.Should().Be(d2);
            (d1 == d2).Should().BeTrue();
        }

        [Fact]
        public void ToString_Should_Return_Empty_When_Null()
        {
            var d = new ItemDescription(null);

            var text = d.ToString();

            text.Should().Be(string.Empty);
        }

        [Fact]
        public void ToString_Should_Return_Value_ToString()
        {
            var text = ValidDescription();
            var d = new ItemDescription(text);

            var result = d.ToString();

            result.Should().Be(text);
        }
    }
}
