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
        public void Ctor_Should_Throw_When_LowerThanMin()
        {
            var value = ItemPrice.MinValue - 1;

            Action act = () => _ = new ItemPrice(value);

            act.Should().Throw<ItemPriceTooSmallException>();
        }

        [Fact]
        public void Value_Equality_Should_Work()
        {
            var value = ItemPrice.MinValue + 1;

            var a = new ItemPrice(value);
            var b = new ItemPrice(value);

            a.Should().Be(b);
            (a == b).Should().BeTrue();
        }

        [Fact]
        public void ToString_Should_Return_Value_ToString()
        {
            var value = ItemPrice.MinValue + 123;
            var p = new ItemPrice(value);

            var text = p.ToString();

            text.Should().Be(value.ToString());
        }
    }
}
