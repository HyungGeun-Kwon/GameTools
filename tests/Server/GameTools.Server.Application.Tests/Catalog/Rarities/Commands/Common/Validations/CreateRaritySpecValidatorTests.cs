using FluentAssertions;
using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Specs;
using GameTools.Server.Application.Catalog.Rarities.Commands.Common.Validations;
using GameTools.Server.Domain.Catalog.Rarities.ValueObjects;
using static GameTools.Server.TestUtilities.Application.Rarities.AppRarityTestData;
using static GameTools.Server.TestUtilities.Domain.Rarities.DomainRarityTestData;

namespace GameTools.Server.Application.Tests.Catalog.Rarities.Commands.Common.Validations
{
    public class CreateRaritySpecValidatorTests
    {
        public static TheoryData<string> ValidGrades() =>
        [
            new string('a', RarityGrade.MinLength),
            new string('a', RarityGrade.MaxLength)
        ];
        [Theory]
        [MemberData(nameof(ValidGrades))]
        public void Validate_Should_Pass_When_Spec_Is_Valid(string grade)
        {
            var validator = new CreateRaritySpecValidator();
            var spec = BuildDefaultCreateRaritySpec(grade: grade);

            var result = validator.Validate(spec);

            result.IsValid.Should().BeTrue();
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
            var validator = new CreateRaritySpecValidator();
            var spec = BuildDefaultCreateRaritySpec(grade: invalidGrade);

            var result = validator.Validate(spec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateRaritySpec.Grade));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(CreateRaritySpec.Grade));
        }

        public static TheoryData<string?> InvalidColorCodes() =>
        [
            null,
            "",
            "   ",
            "FFFFFF",   // missing #
            "#FFFFF",   // wrong length
            "#ZZZZZZ"   // invalid hex
        ];

        [Theory]
        [MemberData(nameof(InvalidColorCodes))]
        public void Validate_Should_Fail_When_ColorCode_Is_Invalid(string? invalidColorCode)
        {
            var validator = new CreateRaritySpecValidator();
            CreateRaritySpec spec;
            spec = new CreateRaritySpec(ValidRarityGradeValue(), invalidColorCode!);

            var result = validator.Validate(spec);

            result.IsValid.Should().BeFalse();

            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateRaritySpec.NormalizedColorCode));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(CreateRaritySpec.NormalizedColorCode));
        }
    }
}
