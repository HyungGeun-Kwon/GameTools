using FluentAssertions;
using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Specs;
using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Validations;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using static GameTools.Server.TestUtilities.Application.Rarities.AppRarityTestData;
using static GameTools.Server.TestUtilities.Domain.Rarities.DomainRarityTestData;

namespace GameTools.Server.Application.Tests.Catalog.Rarities.Commands.Common.Validations
{
    public class UpdateRaritySpecValidatorTests
    {
        public static TheoryData<string> ValidGrades() =>
        [
            new string('A', RarityGrade.MinLength),
            new string('A', RarityGrade.MaxLength)
        ];
        [Theory]
        [MemberData(nameof(ValidGrades))]
        public void Validate_Should_Pass_When_Spec_Is_Valid(string grade)
        {
            var validator = new UpdateRaritySpecValidator();
            var spec = BuildDefaultUpdateRaritySpec(grade: grade);

            var result = validator.Validate(spec);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Id_Is_Empty()
        {
            var validator = new UpdateRaritySpecValidator();
            var spec = BuildDefaultUpdateRaritySpec(id: Guid.Empty);

            var result = validator.Validate(spec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateRaritySpec.Id));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(UpdateRaritySpec.Id));
        }

        public static TheoryData<string> InvalidGrades() =>
        [
            string.Empty,
            "   ",
            "\t\r\n",
            new string('A', RarityGrade.MinLength - 1),
            new string('A', RarityGrade.MaxLength + 1)
        ];

        [Theory]
        [MemberData(nameof(InvalidGrades))]
        public void Validate_Should_Fail_When_Grade_Is_Invalid(string invalidGrade)
        {
            var validator = new UpdateRaritySpecValidator();
            var spec = BuildDefaultUpdateRaritySpec(grade: invalidGrade);

            var result = validator.Validate(spec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateRaritySpec.Grade));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(UpdateRaritySpec.Grade));
        }

        public static TheoryData<string?> InvalidColorCodes() =>
        [
            null,
            "",
            "   ",
            "FFFFFF",
            "#FFFFF",
            "#ZZZZZZ"
        ];
        [Theory]
        [MemberData(nameof(InvalidColorCodes))]
        public void Validate_Should_Fail_When_ColorCode_Is_Invalid(string? invalidColorCode)
        {
            var validator = new UpdateRaritySpecValidator();
            var spec = new UpdateRaritySpec(
                Id: Guid.NewGuid(),
                Grade: ValidRarityGradeValue(),
                ColorCode: invalidColorCode!,
                RowVersion: ValidRarityRowVersion());

            var result = validator.Validate(spec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateRaritySpec.NormalizedColorCode));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(UpdateRaritySpec.NormalizedColorCode));
        }

        public static TheoryData<byte[]> InvalidRowVersions() => [null!, []];
        [Theory]
        [MemberData(nameof(InvalidRowVersions))]
        public void Validate_Should_Fail_When_RowVersion_Is_Null_Or_Empty(byte[] invalidRowVersion)
        {
            var validator = new UpdateRaritySpecValidator();
            var spec = new UpdateRaritySpec(
                Id: Guid.NewGuid(),
                Grade: ValidRarityGradeValue(),
                ColorCode: ValidRarityColorCodeValue(),
                RowVersion: invalidRowVersion);

            var result = validator.Validate(spec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateRaritySpec.RowVersion));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(UpdateRaritySpec.RowVersion));
        }
    }
}
