using FluentAssertions;
using FluentValidation.Results;
using GameTools.Server.Application.Catalog.Items.Commands.UpdateItem;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Validations;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;

namespace GameTools.Server.Application.Tests.Catalog.Items.Commands.UpdateItem
{
    public class UpdateItemValidatorTests
    {
        private static UpdateItemValidator CreateValidator()
            => new(new UpdateItemSpecValidator());

        [Fact]
        public void Validate_Should_Fail_When_Spec_Is_Null()
        {
            var validator = CreateValidator();
            var command = new UpdateItemCommand(null!);

            ValidationResult result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateItemCommand.Spec));
        }

        [Fact]
        public void Validate_Should_Pass_When_Spec_Is_Valid()
        {
            var validator = CreateValidator();
            var spec = BuildDefaultUpdateItemSpec();
            var command = new UpdateItemCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Spec_Is_Invalid()
        {
            var validator = CreateValidator();

            var spec = BuildDefaultUpdateItemSpec(name: string.Empty);
            var command = new UpdateItemCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Spec.Name");
        }
    }
}
