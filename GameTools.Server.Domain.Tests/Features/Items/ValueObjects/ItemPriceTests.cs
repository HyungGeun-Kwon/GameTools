using FluentAssertions;
using GameTools.Server.Domain.Features.Items.Exceptions;
using GameTools.Server.Domain.Features.Items.ValueObjects;

namespace GameTools.Server.Domain.Tests.Features.Items.ValueObjects
{
    public class ItemPriceTests
    {
        [Fact]
        public void Ctor_Should_Allow_MinValue()
        {
            var price = new ItemPrice(ItemPrice.MinValue);

            price.Value.Should().Be(ItemPrice.MinValue);
        }

        [Fact]
        public void Ctor_Should_Allow_PositiveValue()
        {
            var price = new ItemPrice(100);

            price.Value.Should().Be(100);
        }

        [Fact]
        public void Ctor_Should_Throw_When_LowerThanMin()
        {
            var value = ItemPrice.MinValue - 1;

            Action act = () => _ = new ItemPrice(value);

            act.Should().Throw<ItemPriceTooSmallException>();
        }

        [Fact]
        public void Value_Equality_Should_Work()
        {
            var a = new ItemPrice(100);
            var b = new ItemPrice(100);

            a.Should().Be(b);
            (a == b).Should().BeTrue();
        }

        [Fact]
        public void ToString_Should_Return_Value_ToString()
        {
            var p = new ItemPrice(1234);

            var text = p.ToString();

            text.Should().Be("1234");
        }
    }
}
