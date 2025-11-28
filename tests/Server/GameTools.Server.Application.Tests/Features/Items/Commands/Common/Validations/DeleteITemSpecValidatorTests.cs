using FluentAssertions;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using GameTools.Server.Application.Features.Items.Commands.Common.Validations;

namespace GameTools.Server.Application.Tests.Features.Items.Commands.Common.Validations
{
    public class DeleteItemSpecValidatorTests
    {
        private static byte[] ValidRowVersion()
            => Convert.FromBase64String("AAAAAAAAAAA=");

        private static DeleteItemSpec BuildValidSpec(
            Guid? id = null,
            byte[]? rowVersion = null)
            => new
            (
                Id: id ?? Guid.NewGuid(),
                RowVersion: rowVersion ?? ValidRowVersion()
            );

        private static DeleteItemSpec BuildSpec(
            Guid id,
            byte[]? rowVersion)
            => new
            (
                Id: id,
                RowVersion: rowVersion!
            );

        [Fact]
        public void Validate_Should_Pass_When_Spec_Is_Valid()
        {
            var validator = new DeleteItemSpecValidator();
            var validSpec = BuildValidSpec();

            var result = validator.Validate(validSpec);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Id_Is_Empty()
        {
            var validator = new DeleteItemSpecValidator();
            var invalidSpec = BuildValidSpec(id: Guid.Empty);

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
            var invalidSpec = BuildSpec(Guid.NewGuid(), invalidRowVersion);

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(DeleteItemSpec.RowVersion));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(DeleteItemSpec.RowVersion));
        }
    }
}
