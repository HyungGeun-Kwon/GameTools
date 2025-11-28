using FluentAssertions;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using GameTools.Server.Application.Features.Items.Commands.Common.Validations;
using GameTools.Server.Domain.Features.Items.ValueObjects;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;

namespace GameTools.Server.Application.Tests.Features.Items.Commands.Common.Validations
{
    public class CreateItemSpecValidatorTests
    {
        public static TheoryData<string> ValidNameSpecs() =>
        [
            new string('a', ItemName.MinLength),
            new string('a', ItemName.MaxLength)
        ];
        [Theory]
        [MemberData(nameof(ValidNameSpecs))]
        public void Validate_Should_Pass_When_Spec_NameMinMaxValue(string validName)
        {
            var validator = new CreateItemSpecValidator();
            var spec = BuildDefaultCreateItemSpec(name: validName);

            var result = validator.Validate(spec);

            result.IsValid.Should().BeTrue();
        }

        public static TheoryData<string> InvalidNames =>
        [
            string.Empty,
            "   ",
            "\t\r\n",
            new string('a', ItemName.MinLength - 1),
            new string('a', ItemName.MaxLength + 1),
        ];
        [Theory]
        [MemberData(nameof(InvalidNames))]
        public void Validate_Should_Fail_When_Name_Is_Invalid(string invalidName)
        {
            var validator = new CreateItemSpecValidator();
            var invalidSpec = BuildDefaultCreateItemSpec(name: invalidName);

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateItemSpec.Name));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(CreateItemSpec.Name));
        }

        [Fact]
        public void Validate_Should_Fail_When_Price_Is_Invalid()
        {
            var validator = new CreateItemSpecValidator();
            var invalidSpec = BuildDefaultCreateItemSpec(price: ItemPrice.MinValue - 1);

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateItemSpec.Price));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(CreateItemSpec.Price));
        }

        [Fact]
        public void Validate_Should_Fail_When_Description_Is_Too_Long()
        {
            var validator = new CreateItemSpecValidator();
            var invalidSpec = BuildDefaultCreateItemSpec(description: new string('a', ItemDescription.MaxLength + 1));

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateItemSpec.Description));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(CreateItemSpec.Description));
        }

        [Fact]
        public void Validate_Should_Fail_When_RarityId_Is_Empty()
        {
            var validator = new CreateItemSpecValidator();
            var invalidSpec = BuildDefaultCreateItemSpec(rarityId: Guid.Empty);

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateItemSpec.RarityId));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(CreateItemSpec.RarityId));
        }
    }
}
