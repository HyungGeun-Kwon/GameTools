using FluentAssertions;
using GameTools.Server.Application.Catalog.Items.Queries.GetItemsPage;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;

namespace GameTools.Server.Application.Tests.Catalog.Items.Queries.GetItemsPage
{
    public class ItemsFilterValidatorTests
    {
        [Fact]
        public void Validate_Should_Pass_When_Search_Is_Null_And_RarityIds_Is_Null()
        {
            var validator = new ItemsFilterValidator();
            var filter = new ItemsFilter(
                Search: null,
                RarityIds: null
            );

            var result = validator.Validate(filter);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Search_Is_Too_Long()
        {
            var validator = new ItemsFilterValidator();
            var tooLong = new string('a', ItemName.MaxLength + 1);

            var filter = new ItemsFilter(
                Search: tooLong,
                RarityIds: null
            );

            var result = validator.Validate(filter);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(ItemsFilter.Search));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(ItemsFilter.Search));
        }

        [Fact]
        public void Validate_Should_Fail_When_RarityIds_Is_Empty_List()
        {
            var validator = new ItemsFilterValidator();
            var filter = new ItemsFilter(
                Search: null,
                RarityIds: []
            );

            var result = validator.Validate(filter);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(ItemsFilter.RarityIds));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(ItemsFilter.RarityIds));
        }

        [Fact]
        public void Validate_Should_Fail_When_RarityIds_Contains_Duplicates()
        {
            var validator = new ItemsFilterValidator();
            var id = Guid.NewGuid();
            var filter = new ItemsFilter(
                Search: null,
                RarityIds: [ id, id ]
            );

            var result = validator.Validate(filter);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(ItemsFilter.RarityIds));
        }

        [Fact]
        public void Validate_Should_Pass_When_RarityIds_Are_Distinct()
        {
            var validator = new ItemsFilterValidator();
            var filter = new ItemsFilter(
                Search: null,
                RarityIds: [ Guid.NewGuid(), Guid.NewGuid() ]
            );

            var result = validator.Validate(filter);

            result.IsValid.Should().BeTrue();
        }
    }
}
