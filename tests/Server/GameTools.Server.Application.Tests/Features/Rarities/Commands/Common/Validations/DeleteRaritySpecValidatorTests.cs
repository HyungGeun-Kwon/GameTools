using FluentAssertions;
using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Specs;
using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Validations;
using static GameTools.Server.TestUtilities.Application.Rarities.AppRarityTestData;

namespace GameTools.Server.Application.Tests.Catalog.Rarities.Commands.Common.Validations
{
    public class DeleteRaritySpecValidatorTests
    {
        [Fact]
        public void Validate_Should_Pass_When_Spec_Is_Valid()
        {
            var validator = new DeleteRaritySpecValidator();
            var spec = BuildDefaultDeleteRaritySpec();

            var result = validator.Validate(spec);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Id_Is_Empty()
        {
            var validator = new DeleteRaritySpecValidator();
            var spec = BuildDefaultDeleteRaritySpec(id: Guid.Empty);

            var result = validator.Validate(spec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(DeleteRaritySpec.Id));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(DeleteRaritySpec.Id));
        }

        public static TheoryData<byte[]> InvalidRowVersions() => [null!, []];
        [Theory]
        [MemberData(nameof(InvalidRowVersions))]
        public void Validate_Should_Fail_When_RowVersion_Is_Null_Or_Empty(byte[] invalidRowVersion)
        {
            var validator = new DeleteRaritySpecValidator();
            var spec = new DeleteRaritySpec(
                Id: Guid.NewGuid(),
                RowVersion: invalidRowVersion);

            var result = validator.Validate(spec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(DeleteRaritySpec.RowVersion));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(DeleteRaritySpec.RowVersion));
        }
    }
}
