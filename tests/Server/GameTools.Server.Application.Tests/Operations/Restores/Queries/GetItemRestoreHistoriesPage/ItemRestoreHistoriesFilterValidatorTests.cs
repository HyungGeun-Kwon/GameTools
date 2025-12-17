using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GameTools.Server.Application.Operations.Restores.Queries.GetItemRestoreHistoriesPage;

namespace GameTools.Server.Application.Tests.Operations.Restores.Queries.GetItemRestoreHistoriesPage
{
    public class ItemRestoreHistoriesFilterValidatorTests
    {
        private static ItemRestoreHistoriesFilterValidator CreateValidator()
            => new();

        [Fact]
        public void Validate_Should_Pass_When_FromUtc_And_ToUtc_Are_Null()
        {
            var validator = CreateValidator();
            var filter = new ItemRestoreHistoriesFilter(
                Actors: null,
                FromUtc: null,
                ToUtc: null,
                DryOnly: null);

            var result = validator.Validate(filter);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Pass_When_Only_FromUtc_Is_Set()
        {
            var validator = CreateValidator();
            var from = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var filter = new ItemRestoreHistoriesFilter(
                Actors: null,
                FromUtc: from,
                ToUtc: null,
                DryOnly: null);

            var result = validator.Validate(filter);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Pass_When_Only_ToUtc_Is_Set()
        {
            var validator = CreateValidator();
            var to = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc);

            var filter = new ItemRestoreHistoriesFilter(
                Actors: null,
                FromUtc: null,
                ToUtc: to,
                DryOnly: null);

            var result = validator.Validate(filter);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Pass_When_DateRange_Is_Valid()
        {
            var validator = CreateValidator();

            var from = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc);

            var filter = new ItemRestoreHistoriesFilter(
                Actors: null,
                FromUtc: from,
                ToUtc: to,
                DryOnly: null);

            var result = validator.Validate(filter);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_FromUtc_Is_Greater_Than_ToUtc()
        {
            var validator = CreateValidator();

            var from = new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc);

            var filter = new ItemRestoreHistoriesFilter(
                Actors: null,
                FromUtc: from,
                ToUtc: to,
                DryOnly: null);

            var result = validator.Validate(filter);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains("FromUtc must be <= ToUtc."));
        }
    }
}
