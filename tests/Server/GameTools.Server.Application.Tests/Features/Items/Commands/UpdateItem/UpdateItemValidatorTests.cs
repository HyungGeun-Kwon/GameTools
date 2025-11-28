using FluentAssertions;
using FluentValidation.Results;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using GameTools.Server.Application.Features.Items.Commands.Common.Validations;
using GameTools.Server.Application.Features.Items.Commands.UpdateItem;
using GameTools.Server.Domain.Features.Items.ValueObjects;

namespace GameTools.Server.Application.Tests.Features.Items.Commands.UpdateItem
{
    public class UpdateItemValidatorTests
    {
        private static string ValidName() => new('a', ItemName.MinLength);

        private static string ValidDescription()
            => new('d', Math.Min(10, ItemDescription.MaxLength));

        private static UpdateItemSpec BuildValidSpec(
            Guid? id = null,
            string? name = null,
            int? price = null,
            string? description = null,
            Guid? rarityId = null,
            byte[]? rowVersion = null)
            => new(
                Id: id ?? Guid.NewGuid(),
                Name: name ?? ValidName(),
                Price: price ?? ItemPrice.MinValue,
                Description: description ?? ValidDescription(),
                RarityId: rarityId ?? Guid.NewGuid(),
                RowVersion: rowVersion ?? [1, 2, 3, 4 ]);

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
            var spec = BuildValidSpec();
            var command = new UpdateItemCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Spec_Is_Invalid()
        {
            var validator = CreateValidator();

            // Name invalid (empty)
            var spec = BuildValidSpec(name: string.Empty);
            var command = new UpdateItemCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Spec.Name");
        }
    }
}
