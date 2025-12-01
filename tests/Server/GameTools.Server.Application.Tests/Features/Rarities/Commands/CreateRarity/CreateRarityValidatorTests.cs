using FluentAssertions;
using FluentValidation.Results;
using GameTools.Server.Application.Features.Rarities.Commands.Common.Validations;
using GameTools.Server.Application.Features.Rarities.Commands.CreateRarity;
using static GameTools.Server.TestUtilities.Application.Rarities.AppRarityTestData;

namespace GameTools.Server.Application.Tests.Features.Rarities.Commands.CreateRarity
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
