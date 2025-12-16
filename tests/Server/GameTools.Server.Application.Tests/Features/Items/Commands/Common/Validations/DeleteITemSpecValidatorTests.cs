using FluentAssertions;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Validations;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;

namespace GameTools.Server.Application.Tests.Catalog.Items.Commands.Common.Validations
{
    public class DeleteItemSpecValidatorTests
    {
        [Fact]
        public void Validate_Should_Pass_When_Spec_Is_Valid()
        {
            var validator = new DeleteItemSpecValidator();
            var validSpec = BuildDefaultDeleteItemSpec();

            var result = validator.Validate(validSpec);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Id_Is_Empty()
        {
            var validator = new DeleteItemSpecValidator();
            var invalidSpec = BuildDefaultDeleteItemSpec(id: Guid.Empty);

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(DeleteItemSpec.Id));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(DeleteItemSpec.Id));
        }

        public static TheoryData<byte[]> InvalidRowVersions() => [null!, []];
        [Theory]
        [MemberData(nameof(InvalidRowVersions))]
        public void Validate_Should_Fail_When_RowVersion_Is_Null_Or_Empty(byte[] invalidRowVersion)
        {
            var validator = new DeleteItemSpecValidator();
            var invalidSpec = new DeleteItemSpec(
                Id: Guid.NewGuid(),
                RowVersion: invalidRowVersion);

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(DeleteItemSpec.RowVersion));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(DeleteItemSpec.RowVersion));
        }
    }
}
