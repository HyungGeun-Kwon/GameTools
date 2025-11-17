using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GameTools.Server.Domain.Features.Items.ValueObjects;

namespace GameTools.Server.Domain.Tests.Features.Items.ValueObjects
{
    public class ItemIdTests
    {
        [Fact]
        public void New_Should_Create_NonEmptyGuid()
        {
            var id = ItemId.New();

            id.Value.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void From_With_EmptyGuid_Should_Throw()
        {
            var empty = Guid.Empty;

            Action act = () => ItemId.From(empty);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void From_With_ValidGuid_Should_Create_ItemId()
        {
            var guid = Guid.NewGuid();


            var id = ItemId.From(guid);

            id.Value.Should().Be(guid);
        }

        [Fact]
        public void Parse_With_ValidString_Should_Create_ItemId()
        {
            var g = Guid.NewGuid();
            var s = g.ToString();

            var id = ItemId.Parse(s);

            id.Value.Should().Be(g);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("not-a-guid")]
        [InlineData("00000000-0000-0000-0000-000000000000")] // Empty Guid
        public void TryParse_InvalidInput_Should_ReturnFalse_And_Null(string? input)
        {
            var success = ItemId.TryParse(input, out var result);

            success.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void TryParse_ValidGuid_Should_ReturnTrue_And_ItemId()
        {
            var g = Guid.NewGuid();
            var s = g.ToString();

            var success = ItemId.TryParse(s, out var result);

            success.Should().BeTrue();
            result.Should().NotBeNull();
            result!.Value.Should().Be(g);
        }

        [Fact]
        public void ToString_Should_Return_Value_ToString()
        {
            var g = Guid.NewGuid();
            var id = ItemId.From(g);

            var text = id.ToString();

            text.Should().Be(g.ToString());
        }
    }
}
