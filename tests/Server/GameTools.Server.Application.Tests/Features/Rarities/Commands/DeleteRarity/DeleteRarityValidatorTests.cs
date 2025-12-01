using FluentAssertions;
using FluentValidation.Results;
using GameTools.Server.Application.Features.Rarities.Commands.Common.Validations;
using GameTools.Server.Application.Features.Rarities.Commands.DeleteRarity;
using static GameTools.Server.TestUtilities.Application.Rarities.AppRarityTestData;

namespace GameTools.Server.Application.Tests.Features.Rarities.Commands.DeleteRarity
{
    public class DeleteRarityValidatorTests
    {
        public static DeleteRarityValidator CreateValidator()
            => new(new DeleteRaritySpecValidator());

        [Fact]
        public void Validate_Should_Fail_When_Spec_Is_Null()
        {
            var validator = CreateValidator();
            var command = new DeleteRarityCommand(null!);

            ValidationResult result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(DeleteRarityCommand.Spec));
        }

        [Fact]
        public void Validate_Should_Pass_When_Spec_Is_Valid()
        {
            var validator = CreateValidator();
            var spec = BuildDefaultDeleteRaritySpec();
            var command = new DeleteRarityCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Spec_Is_Invalid()
        {
            var validator = CreateValidator();
            // RowVersion invalid (empty)
            var spec = BuildDefaultDeleteRaritySpec(rowVersion: []);
            var command = new DeleteRarityCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Spec.RowVersion");
        }
    }
}
