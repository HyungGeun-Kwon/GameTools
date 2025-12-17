using FluentAssertions;
using GameTools.Server.Application.Auditing.Queries.GetItemAuditPage;

namespace GameTools.Server.Application.Tests.Auditing.Queries.GetItemAuditPage
{
    public class ItemAuditFilterValidatorTests
    {
        private static ItemAuditFilterValidator CreateValidator()
            => new();

        [Fact]
        public void Validate_Should_Pass_When_Actions_Is_Null()
        {
            var validator = CreateValidator();
            var filter = new ItemAuditFilter(
                ItemId: null,
                Actions: null,
                FromUtc: null,
                ToUtc: null);

            var result = validator.Validate(filter);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Pass_When_Actions_Is_Empty()
        {
            var validator = CreateValidator();
            var filter = new ItemAuditFilter(
                ItemId: null,
                Actions: [],
                FromUtc: null,
                ToUtc: null);

            var result = validator.Validate(filter);

            result.IsValid.Should().BeTrue();
        }

        public static TheoryData<string[]> ValidActions() =>
        [
            [ "INSERT" ],
            [ "UPDATE" ],
            [ "DELETE" ],
            [ "INSERT", "UPDATE", "DELETE"]
        ];
        [Theory]
        [MemberData(nameof(ValidActions))]
        public void Validate_Should_Pass_When_Actions_Are_Allowed(string[] actions)
        {
            var validator = CreateValidator();
            var filter = new ItemAuditFilter(
                ItemId: null,
                Actions: actions,
                FromUtc: null,
                ToUtc: null);

            var result = validator.Validate(filter);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_Actions_Contain_Not_Allowed_Value()
        {
            var validator = CreateValidator();
            var filter = new ItemAuditFilter(
                ItemId: null,
                Actions: [ "INSERT", "UNKNOWN" ],
                FromUtc: null,
                ToUtc: null);

            var result = validator.Validate(filter);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Action must be one of"));
        }

        [Fact]
        public void Validate_Should_Pass_When_DateRange_Is_Valid()
        {
            var validator = CreateValidator();

            var from = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc);

            var filter = new ItemAuditFilter(
                ItemId: null,
                Actions: null,
                FromUtc: from,
                ToUtc: to);

            var result = validator.Validate(filter);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_FromUtc_Is_Greater_Than_ToUtc()
        {
            var validator = CreateValidator();

            var from = new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc);

            var filter = new ItemAuditFilter(
                ItemId: null,
                Actions: null,
                FromUtc: from,
                ToUtc: to);

            var result = validator.Validate(filter);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains("FromUtc must be <= ToUtc."));
        }

        [Fact]
        public void Validate_Should_Pass_When_Only_FromUtc_Is_Set()
        {
            var validator = CreateValidator();

            var from = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var filter = new ItemAuditFilter(
                ItemId: null,
                Actions: null,
                FromUtc: from,
                ToUtc: null);

            var result = validator.Validate(filter);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Pass_When_Only_ToUtc_Is_Set()
        {
            var validator = CreateValidator();

            var to = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc);

            var filter = new ItemAuditFilter(
                ItemId: null,
                Actions: null,
                FromUtc: null,
                ToUtc: to);

            var result = validator.Validate(filter);

            result.IsValid.Should().BeTrue();
        }
    }
}
