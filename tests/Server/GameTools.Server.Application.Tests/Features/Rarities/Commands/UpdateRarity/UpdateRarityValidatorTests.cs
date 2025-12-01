using FluentAssertions;
using FluentValidation.Results;
using GameTools.Server.Application.Features.Rarities.Commands.Common.Validations;
using GameTools.Server.Application.Features.Rarities.Commands.UpdateRarity;
using static GameTools.Server.TestUtilities.Application.Rarities.AppRarityTestData;

namespace GameTools.Server.Application.Tests.Features.Rarities.Commands.UpdateRarity
{
    public class UpdateRarityValidatorTests
    {
        private static UpdateRarityValidator CreateValidator()
            => new(new UpdateRaritySpecValidator());

        [Fact]
        public void Validate_Should_Fail_When_Spec_Is_Null()
        {
            var validator = CreateValidator();
            var command = new UpdateRarityCommand(null!);

            ValidationResult result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateRarityCommand.Spec));
        }

        [Fact]
        public void Validate_Should_Pass_When_Spec_Is_Valid()
        {
            var validator = CreateValidator();
            var spec = BuildDefaultUpdateRaritySpec();
            var command = new UpdateRarityCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Spec_Is_Invalid()
        {
            var validator = CreateValidator();

            var spec = BuildDefaultUpdateRaritySpec(grade: string.Empty);
            var command = new UpdateRarityCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Spec.Grade");
        }
    }
}
