using FluentAssertions;
using GameTools.Server.Application.Features.Items.Commands.BulkUpdateItems;
using GameTools.Server.Application.Features.Items.Commands.Common.Validations;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;

namespace GameTools.Server.Application.Tests.Features.Items.Commands.BulkUpdateItems
{
    public class BulkUpdateItemsValidatorTests
    {
        [Fact]
        public void Validate_Should_Fail_When_Specs_Is_Null()
        {
            var specValidator = new UpdateItemSpecValidator();
            var validator = new BulkUpdateItemsValidator(specValidator);

            var command = new BulkUpdateItemsCommand(null!);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(BulkUpdateItemsCommand.Specs));
        }

        [Fact]
        public void Validate_Should_Pass_When_All_Specs_Are_Valid()
        {
            var specValidator = new UpdateItemSpecValidator();
            var validator = new BulkUpdateItemsValidator(specValidator);

            var specs = new[]
            {
                BuildDefaultUpdateItemSpec(),
                BuildDefaultUpdateItemSpec()
            };

            var command = new BulkUpdateItemsCommand(specs);

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Any_Spec_Is_Invalid()
        {
            var specValidator = new UpdateItemSpecValidator();
            var validator = new BulkUpdateItemsValidator(specValidator);

            var invalidSpec = BuildDefaultUpdateItemSpec(id: Guid.Empty);
            var command = new BulkUpdateItemsCommand([invalidSpec]);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Specs[0].Id");
        }
    }
}
