using FluentAssertions;
using FluentValidation.Results;
using GameTools.Server.Application.Catalog.Rarities.Commands.CreateRarity;
using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Validations;
using static GameTools.Server.TestUtilities.Application.Rarities.AppRarityTestData;

namespace GameTools.Server.Application.Tests.Catalog.Rarities.Commands.CreateRarity
{
    public class CreateRarityValidatorTests
    {
        private static CreateRarityValidator CreateValidator()
            => new(new CreateRaritySpecValidator());

        [Fact]
        public void Validate_Should_Fail_When_Spec_Is_Null()
        {
            var validator = CreateValidator();
            var command = new CreateRarityCommand(null!);

            ValidationResult result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateRarityCommand.Spec));
        }

        [Fact]
        public void Validate_Should_Pass_When_Spec_Is_Valid()
        {
            var validator = CreateValidator();
            var spec = BuildDefaultCreateRaritySpec();
            var command = new CreateRarityCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Spec_Is_Invalid()
        {
            var validator = CreateValidator();

            var spec = BuildDefaultCreateRaritySpec(grade: string.Empty);
            var command = new CreateRarityCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Spec.Grade");
        }
    }
}
