using FluentAssertions;
using GameTools.Server.Application.Catalog.Rarities.Queries.GetRarityById;

namespace GameTools.Server.Application.Tests.Catalog.Rarities.Queries.GetRarityById
{
    public class GetRarityByIdValidatorTests
    {
        [Fact]
        public void Validate_Should_Pass_When_Id_Is_NotEmpty()
        {
            var validator = new GetRarityByIdValidator();
            var query = new GetRarityByIdQuery(Guid.NewGuid());

            var result = validator.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Id_Is_Empty()
        {
            var validator = new GetRarityByIdValidator();
            var query = new GetRarityByIdQuery(Guid.Empty);

            var result = validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(GetRarityByIdQuery.Id));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(GetRarityByIdQuery.Id));
        }
    }
}
