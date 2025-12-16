using FluentAssertions;
using GameTools.Server.Application.Catalog.Items.Queries.GetItemById;

namespace GameTools.Server.Application.Tests.Catalog.Items.Queries.GetItemById
{
    public class GetItemByIdValidatorTests
    {
        [Fact]
        public void Validate_Should_Pass_When_Id_Is_NotEmpty()
        {
            var validator = new GetItemByIdValidator();
            var query = new GetItemByIdQuery(Guid.NewGuid());

            var result = validator.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Id_Is_Empty()
        {
            var validator = new GetItemByIdValidator();
            var query = new GetItemByIdQuery(Guid.Empty);

            var result = validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(GetItemByIdQuery.Id));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(GetItemByIdQuery.Id));
        }
    }
}
