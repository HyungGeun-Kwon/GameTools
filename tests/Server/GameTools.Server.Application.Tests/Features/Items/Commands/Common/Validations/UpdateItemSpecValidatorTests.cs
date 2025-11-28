using FluentAssertions;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using GameTools.Server.Application.Features.Items.Commands.Common.Validations;
using GameTools.Server.Domain.Features.Items.ValueObjects;

namespace GameTools.Server.Application.Tests.Features.Items.Commands.Common.Validations
{
    public class UpdateItemSpecValidatorTests
    {
        private static string ValidName() => new('a', ItemName.MinLength);
        private static string ValidDescription()
            => new('a', ItemDescription.MaxLength);
        private static byte[] ValidRowVersion()
            => Convert.FromBase64String("AAAAAAAAAAA=");

        private static UpdateItemSpec BuildValidSpec(
            Guid? id = null,
            string? name = null,
            int? price = null,
            string? description = null,
            Guid? rarityId = null,
            byte[]? rowVersion = null)
        => new
        (
            Id: id ?? Guid.NewGuid(),
            Name: name ?? ValidName(),
            Price: price ?? ItemPrice.MinValue,
            Description: description ?? ValidDescription(),
            RarityId: rarityId ?? Guid.NewGuid(),
            RowVersion: rowVersion ?? ValidRowVersion()
        );

        private static UpdateItemSpec BuildSpec(
            Guid id,
            string? name,
            int price,
            string? description,
            Guid rarityId,
            byte[]? rowVersion)
            => new
            (
                Id: id,
                Name: name!,
                Price: price,
                Description: description!,
                RarityId: rarityId,
                RowVersion: rowVersion!
            );

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
            var spec = BuildValidSpec(name: validName);

            var result = validator.Validate(spec);

            result.IsValid.Should().BeTrue();
        }


        [Fact]
        public void Validate_Should_Fail_When_Id_Is_Empty()
        {
            var validator = new UpdateItemSpecValidator();
            var invalidSpec = BuildValidSpec(id: Guid.Empty);

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
            var invalidSpec = BuildValidSpec(name: invalidName);

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateItemSpec.Name));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(UpdateItemSpec.Name));
        }

        [Fact]
        public void Validate_Should_Fail_When_Price_Is_Invalid()
        {
            var validator = new UpdateItemSpecValidator();
            var invalidSpec = BuildValidSpec(price: ItemPrice.MinValue - 1);

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateItemSpec.Price));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(UpdateItemSpec.Price));
        }

        [Fact]
        public void Validate_Should_Fail_When_Description_Is_Too_Long()
        {
            var validator = new UpdateItemSpecValidator();
            var invalidSpec = BuildValidSpec(description: new string('a', ItemDescription.MaxLength + 1));

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateItemSpec.Description));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(UpdateItemSpec.Description));
        }

        [Fact]
        public void Validate_Should_Fail_When_RarityId_Is_Empty()
        {
            var validator = new UpdateItemSpecValidator();
            var invalidSpec = BuildValidSpec(rarityId: Guid.Empty);

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
            var invalidSpec = BuildSpec(
                id: Guid.NewGuid(),
                name: ValidName(),
                price: ItemPrice.MinValue,
                description: ValidDescription(),
                rarityId: Guid.NewGuid(),
                rowVersion: invalidRowVersion);

            var result = validator.Validate(invalidSpec);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateItemSpec.RowVersion));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(UpdateItemSpec.RowVersion));
        }
    }
}
