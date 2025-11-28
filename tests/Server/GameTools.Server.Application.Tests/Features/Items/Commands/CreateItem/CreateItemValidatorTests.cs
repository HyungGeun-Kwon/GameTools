using FluentAssertions;
using FluentValidation.Results;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using GameTools.Server.Application.Features.Items.Commands.Common.Validations;
using GameTools.Server.Application.Features.Items.Commands.CreateItem;
using GameTools.Server.Domain.Features.Items.ValueObjects;

namespace GameTools.Server.Application.Tests.Features.Items.Commands.CreateItem
{
    public class CreateItemValidatorTests
    {
        private static string ValidName() => new('a', ItemName.MinLength);
        private static string ValidDescription()
            => new('a', ItemDescription.MaxLength);

        private static CreateItemSpec BuildValidSpec(
            string? name = null,
            int? price = null,
            string? description = null,
            Guid? rarityId = null)
            => new
            (
                Name: name ?? ValidName(),
                Price: price ?? ItemPrice.MinValue,
                Description: description ?? ValidDescription(),
                RarityId: rarityId ?? Guid.NewGuid()
            );

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
            var spec = BuildValidSpec();
            var command = new CreateItemCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Spec_Is_Invalid()
        {
            var validator = CreateValidator();

            // Name invalid (empty)
            var spec = BuildValidSpec(name: string.Empty);
            var command = new CreateItemCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Spec.Name");
        }
    }
}
