using FluentAssertions;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;

namespace GameTools.Server.Domain.Tests.Catalog.Rarities.ValueObjects
{
    public class RarityIdTests
    {
        [Fact]
        public void New_Should_Create_NonEmptyGuid()
        {
            var id = RarityId.New();

            id.Value.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void From_With_EmptyGuid_Should_Throw()
        {
            var empty = Guid.Empty;

            Action act = () => RarityId.From(empty);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void From_With_ValidGuid_Should_Create_RarityId()
        {
            var guid = Guid.NewGuid();

            var id = RarityId.From(guid);

            id.Value.Should().Be(guid);
        }

        [Fact]
        public void Parse_With_ValidString_Should_Create_RarityId()
        {
            var g = Guid.NewGuid();
            var s = g.ToString();

            var id = RarityId.Parse(s);

            id.Value.Should().Be(g);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("not-a-guid")]
        [InlineData("00000000-0000-0000-0000-000000000000")] // Empty Guid
        public void TryParse_InvalidInput_Should_ReturnFalse_And_Null(string? input)
        {
            var success = RarityId.TryParse(input, out var result);

            success.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void TryParse_ValidGuid_Should_ReturnTrue_And_RarityId()
        {
            var g = Guid.NewGuid();
            var s = g.ToString();

            var success = RarityId.TryParse(s, out var result);

            success.Should().BeTrue();
            result.Should().NotBeNull();
            result!.Value.Should().Be(g);
        }

        [Fact]
        public void ToString_Should_Return_Value_ToString()
        {
            var g = Guid.NewGuid();
            var id = RarityId.From(g);

            var text = id.ToString();

            text.Should().Be(g.ToString());
        }
    }
}
