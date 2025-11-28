using FluentAssertions;
using FluentValidation.Results;
using GameTools.Server.Application.Features.Items.Commands.Common.Validations;
using GameTools.Server.Application.Features.Items.Commands.DeleteItem;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;

namespace GameTools.Server.Application.Tests.Features.Items.Commands.DeleteItem
{
    public class DeleteItemValidatorTests
    {
        private static DeleteItemValidator CreateValidator()
            => new(new DeleteItemSpecValidator());

        [Fact]
        public void Validate_Should_Fail_When_Spec_Is_Null()
        {
            var validator = CreateValidator();
            var command = new DeleteItemCommand(null!);

            ValidationResult result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(DeleteItemCommand.Spec));
        }

        [Fact]
        public void Validate_Should_Pass_When_Spec_Is_Valid()
        {
            var validator = CreateValidator();
            var spec = BuildDefaultDeleteItemSpec();
            var command = new DeleteItemCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Spec_Is_Invalid()
        {
            var validator = CreateValidator();
            // RowVersion invalid (empty)
            var spec = BuildDefaultDeleteItemSpec(rowVersion: []);
            var command = new DeleteItemCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Spec.RowVersion");
        }
    }
}
