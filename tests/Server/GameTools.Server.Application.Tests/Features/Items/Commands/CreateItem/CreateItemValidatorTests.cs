using FluentAssertions;
using FluentValidation.Results;
using GameTools.Server.Application.Catalog.Items.Commands.CreateItem;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Validations;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;

namespace GameTools.Server.Application.Tests.Catalog.Items.Commands.CreateItem
{
    public class CreateItemValidatorTests
    {
        private static CreateItemValidator CreateValidator()
            => new(new CreateItemSpecValidator());

        [Fact]
        public void Validate_Should_Fail_When_Spec_Is_Null()
        {
            var validator = CreateValidator();
            var command = new CreateItemCommand(null!);

            ValidationResult result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateItemCommand.Spec));
        }

        [Fact]
        public void Validate_Should_Pass_When_Spec_Is_Valid()
        {
            var validator = CreateValidator();
            var spec = BuildDefaultCreateItemSpec();
            var command = new CreateItemCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Spec_Is_Invalid()
        {
            var validator = CreateValidator();

            var spec = BuildDefaultCreateItemSpec(name: string.Empty);
            var command = new CreateItemCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Spec.Name");
        }
    }
}
