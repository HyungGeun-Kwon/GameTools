using FluentAssertions;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Validations;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;
using static GameTools.Server.TestUtilities.Application.Items.AppItemTestData;
using static GameTools.Server.TestUtilities.Domain.Items.DomainItemTestData;

namespace GameTools.Server.Application.Tests.Catalog.Items.Commands.Common.Validations
{
    public class UpdateItemSpecValidatorTests
    {
        public static TheoryData<string> ValidNameSpecs() =>
        [
            new string('a', ItemName.MinLength),
            new string('a', ItemName.MaxLength)
        ];
        [Theory]
        [MemberData(nameof(ValidNameSpecs))]
        public void Validate_Should_Pass_When_Spec_Is_Valid(string validName)
        {
            var validator = new UpdateItemSpecValidator();
            var spec = BuildDefaultUpdateItemSpec(name: validName);

            var result = validator.Validate(spec);

            result.IsValid.Should().BeTrue();
        }


        [Fact]
        public void Validate_Should_Fail_When_Id_Is_Empty()
        {
            var validator = new UpdateItemSpecValidator();
            var invalidSpec = BuildDefaultUpdateItemSpec(id: Guid.Empty);

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateItemSpec.Id));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(UpdateItemSpec.Id));
        }


        public static TheoryData<string> InvalidNames() =>
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
            var validator = new UpdateItemSpecValidator();
            var invalidSpec = BuildDefaultUpdateItemSpec(name: invalidName);

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateItemSpec.Name));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(UpdateItemSpec.Name));
        }

        [Fact]
        public void Validate_Should_Fail_When_Price_Is_Invalid()
        {
            var validator = new UpdateItemSpecValidator();
            var invalidSpec = BuildDefaultUpdateItemSpec(price: ItemPrice.MinValue - 1);

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateItemSpec.Price));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(UpdateItemSpec.Price));
        }

        [Fact]
        public void Validate_Should_Fail_When_Description_Is_Too_Long()
        {
            var validator = new UpdateItemSpecValidator();
            var invalidSpec = BuildDefaultUpdateItemSpec(description: new string('a', ItemDescription.MaxLength + 1));

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateItemSpec.Description));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(UpdateItemSpec.Description));
        }

        [Fact]
        public void Validate_Should_Fail_When_RarityId_Is_Empty()
        {
            var validator = new UpdateItemSpecValidator();
            var invalidSpec = BuildDefaultUpdateItemSpec(rarityId: Guid.Empty);

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateItemSpec.RarityId));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(UpdateItemSpec.RarityId));
        }

        public static TheoryData<byte[]> InvalidRowVersions() => [null!, []];
        [Theory]
        [MemberData(nameof(InvalidRowVersions))]
        public void Validate_Should_Fail_When_RowVersion_Is_Null_Or_Empty(byte[] invalidRowVersion)
        {
            var validator = new UpdateItemSpecValidator();
            var invalidSpec = new UpdateItemSpec(
                Id: Guid.NewGuid(),
                Name: ValidItemNameValue(),
                Price: ValidItemPriceValue(),
                Description: ValidItemDescriptionValue(),
                RarityId: Guid.NewGuid(),
                RowVersion: invalidRowVersion);

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateItemSpec.RowVersion));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(UpdateItemSpec.RowVersion));
        }
    }
}
