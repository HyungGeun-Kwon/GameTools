using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.Results;
using GameTools.Server.Application.Features.Restores.Commands.RestoreItems;
using static GameTools.Server.TestUtilities.Application.Restores.AppRestoreItemTestData;

namespace GameTools.Server.Application.Tests.Features.Restores.Commands.RestoreItems
{
    public class RestoreItemsValidatorTests
    {
        private static RestoreItemsValidator CreateValidator()
            => new(new RestoreItemsSpecValidator());

        [Fact]
        public void Validate_Should_Fail_When_Spec_Is_Null()
        {
            var validator = CreateValidator();
            var command = new RestoreItemsCommand(null!);

            ValidationResult result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RestoreItemsCommand.Spec));
        }

        [Fact]
        public void Validate_Should_Pass_When_Spec_Is_Valid()
        {
            var validator = CreateValidator();
            var spec = BuildDefaultRestoreItemsSpec();
            var command = new RestoreItemsCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_ItemIdsSpec_Is_Invalid()
        {
            var validator = CreateValidator();
            var spec = BuildDefaultRestoreItemsSpec(itemIds: []);
            var command = new RestoreItemsCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Spec.ItemIds");
        }

        [Fact]
        public void Validate_Should_Fail_When_AsOfUtcSpec_Is_Invalid()
        {
            var validator = CreateValidator();
            var spec = BuildDefaultRestoreItemsSpec(asOfUtc: DateTime.UtcNow.AddHours(1));
            var command = new RestoreItemsCommand(spec);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Spec.AsOfUtc");
        }
    }
}
