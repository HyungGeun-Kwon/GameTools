using FluentAssertions;
using GameTools.Server.Application.Catalog.Items.Commands.BulkDeleteItems;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Validations;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;

namespace GameTools.Server.Application.Tests.Catalog.Items.Commands.BulkDeleteItems
{
    public class BulkDeleteItemsValidatorTests
    {
        [Fact]
        public void Validate_Should_Fail_When_Specs_Is_Null()
        {
            var specValidator = new DeleteItemSpecValidator();
            var validator = new BulkDeleteItemsValidator(specValidator);

            var command = new BulkDeleteItemsCommand(null!);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(BulkDeleteItemsCommand.Specs));
        }

        [Fact]
        public void Validate_Should_Pass_When_All_Specs_Are_Valid()
        {
            var specValidator = new DeleteItemSpecValidator();
            var validator = new BulkDeleteItemsValidator(specValidator);

            var specs = new[]
            {
                BuildDefaultDeleteItemSpec(),
                BuildDefaultDeleteItemSpec()
            };

            var command = new BulkDeleteItemsCommand(specs);

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Any_Spec_Is_Invalid()
        {
            var specValidator = new DeleteItemSpecValidator();
            var validator = new BulkDeleteItemsValidator(specValidator);

            var invalidSpec = BuildDefaultDeleteItemSpec(id: Guid.Empty);
            var command = new BulkDeleteItemsCommand([invalidSpec]);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Specs[0].Id");
        }
    }
}
