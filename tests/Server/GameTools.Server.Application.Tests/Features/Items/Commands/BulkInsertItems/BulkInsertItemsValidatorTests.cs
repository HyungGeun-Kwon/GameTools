using FluentAssertions;
using GameTools.Server.Application.Features.Items.Commands.BulkInsertItems;
using GameTools.Server.Application.Features.Items.Commands.Common.Validations;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;

namespace GameTools.Server.Application.Tests.Features.Items.Commands.BulkInsertItems
{
    public class BulkInsertItemsValidatorTests
    {
        [Fact]
        public void Validate_Should_Fail_When_Specs_Is_Null()
        {
            var specValidator = new CreateItemSpecValidator();
            var validator = new BulkInsertItemsValidator(specValidator);

            var command = new BulkInsertItemsCommand(null!);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(BulkInsertItemsCommand.Specs));
        }

        [Fact]
        public void Validate_Should_Pass_When_Specs_Is_Empty()
        {
            var specValidator = new CreateItemSpecValidator();
            var validator = new BulkInsertItemsValidator(specValidator);

            var command = new BulkInsertItemsCommand([]);

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Pass_When_All_Specs_Are_Valid()
        {
            var specValidator = new CreateItemSpecValidator();
            var validator = new BulkInsertItemsValidator(specValidator);

            var specs = new[]
            {
                BuildDefaultCreateItemSpec(),
                BuildDefaultCreateItemSpec(),
            };

            var command = new BulkInsertItemsCommand(specs);

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Any_Spec_Is_Invalid()
        {
            var specValidator = new CreateItemSpecValidator();
            var validator = new BulkInsertItemsValidator(specValidator);

            // Name이 빈 문자열이라서 Spec 레벨에서 Invalid
            var invalidSpec = BuildDefaultCreateItemSpec(name: string.Empty);
            var specs = new[] { invalidSpec };

            var command = new BulkInsertItemsCommand(specs);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();

            result.Errors.Should().Contain(e => e.PropertyName == "Specs[0].Name");
        }
    }
}
