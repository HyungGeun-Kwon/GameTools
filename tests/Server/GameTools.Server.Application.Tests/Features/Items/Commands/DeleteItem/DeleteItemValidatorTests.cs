using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.Results;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using GameTools.Server.Application.Features.Items.Commands.Common.Validations;
using GameTools.Server.Application.Features.Items.Commands.DeleteItem;

namespace GameTools.Server.Application.Tests.Features.Items.Commands.DeleteItem
{
    public class DeleteItemValidatorTests
    {
        private static DeleteItemSpec BuildValidSpec(
            Guid? id = null,
            byte[]? rowVersion = null)
            => new(
                Id: id ?? Guid.NewGuid(),
                RowVersion: rowVersion ?? [ 1, 2, 3, 4 ]);

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
            var spec = BuildValidSpec();
            var command = new DeleteItemCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Spec_Is_Invalid()
        {
            var validator = CreateValidator();
            // RowVersion invalid (empty)
            var spec = BuildValidSpec(rowVersion: Array.Empty<byte>());
            var command = new DeleteItemCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Spec.RowVersion");
        }
    }
}
