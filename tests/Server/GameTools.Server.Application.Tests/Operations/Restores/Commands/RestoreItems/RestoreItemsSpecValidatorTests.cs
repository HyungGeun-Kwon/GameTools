using FluentAssertions;
using GameTools.Server.Application.Operations.Restores.Commands.RestoreItems;
using static GameTools.Server.TestUtilities.Application.Restores.AppRestoreItemTestData;

namespace GameTools.Server.Application.Tests.Operations.Restores.Commands.RestoreItems
{
    public class RestoreItemsSpecValidatorTests
    {
        private static RestoreItemsSpecValidator CreateValidator() => new();

        [Fact]
        public void Validate_Should_Pass_When_Spec_Is_Valid_With_Null_ItemIds()
        {
            var validator = CreateValidator();
            var spec = BuildDefaultRestoreItemsSpec();

            var result = validator.Validate(spec);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Pass_When_Spec_Is_Valid_With_ItemIds()
        {
            var validator = CreateValidator();
            var spec = BuildDefaultRestoreItemsSpec(itemIds: [Guid.NewGuid(), Guid.NewGuid()]);

            var result = validator.Validate(spec);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_AsOfUtc_Is_Default()
        {
            var validator = CreateValidator();
            var spec = new RestoreItemsSpec(
                AsOfUtc: default,
                ItemIds: null,
                Notes: "note",
                DryRun: false);

            var result = validator.Validate(spec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RestoreItemsSpec.AsOfUtc));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(RestoreItemsSpec.AsOfUtc));
        }

        [Fact]
        public void Validate_Should_Fail_When_AsOfUtc_Is_In_Future()
        {
            var validator = new RestoreItemsSpecValidator();
            var spec = BuildDefaultRestoreItemsSpec(asOfUtc: DateTime.UtcNow.AddHours(1));

            var result = validator.Validate(spec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RestoreItemsSpec.AsOfUtc));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(RestoreItemsSpec.AsOfUtc));
        }

        [Fact]
        public void Validate_Should_Fail_When_ItemIds_Is_Empty_Collection()
        {
            var validator = new RestoreItemsSpecValidator();
            var spec = BuildDefaultRestoreItemsSpec(itemIds: []);

            var result = validator.Validate(spec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RestoreItemsSpec.ItemIds));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(RestoreItemsSpec.ItemIds));
        }

        [Fact]
        public void Validate_Should_Fail_When_ItemIds_Contains_Empty_Guid()
        {
            var validator = new RestoreItemsSpecValidator();
            var spec = BuildDefaultRestoreItemsSpec(itemIds: [Guid.Empty]);

            var result = validator.Validate(spec);

            result.IsValid.Should().BeFalse();

            result.Errors.Should().Contain(e =>
                e.PropertyName.StartsWith(nameof(RestoreItemsSpec.ItemIds), StringComparison.Ordinal));
        }
    }
}
