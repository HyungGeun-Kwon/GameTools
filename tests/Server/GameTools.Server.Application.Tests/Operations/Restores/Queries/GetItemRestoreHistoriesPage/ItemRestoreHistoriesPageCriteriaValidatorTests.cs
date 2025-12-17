using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Operations.Restores.Queries.GetItemRestoreHistoriesPage;
using Moq;

namespace GameTools.Server.Application.Tests.Operations.Restores.Queries.GetItemRestoreHistoriesPage
{
    public class ItemRestoreHistoriesPageCriteriaValidatorTests
    {
        private static ItemRestoreHistoriesPageCriteriaValidator CreateValidator(
            out Mock<IValidator<Pagination>> paginationValidatorMock,
            out Mock<IValidator<ItemRestoreHistoriesFilter>> filterValidatorMock)
        {
            paginationValidatorMock = new Mock<IValidator<Pagination>>();
            paginationValidatorMock
                .Setup(v => v.Validate(It.IsAny<ValidationContext<Pagination>>()))
                .Returns(new ValidationResult());

            filterValidatorMock = new Mock<IValidator<ItemRestoreHistoriesFilter>>();
            filterValidatorMock
                .Setup(v => v.Validate(It.IsAny<ValidationContext<ItemRestoreHistoriesFilter>>()))
                .Returns(new ValidationResult());

            return new ItemRestoreHistoriesPageCriteriaValidator(
                paginationValidatorMock.Object,
                filterValidatorMock.Object);
        }

        [Fact]
        public void Validate_Should_Pass_When_Criteria_Is_Valid()
        {
            var validator = CreateValidator(
                out var paginationValidatorMock,
                out var filterValidatorMock);

            var pagination = new Pagination();
            var filter = new ItemRestoreHistoriesFilter(
                Actors: ["tester"],
                FromUtc: null,
                ToUtc: null,
                DryOnly: true);

            var criteria = new ItemRestoreHistoriesPageCriteria(
                Pagination: pagination,
                Filter: filter,
                SortBy: nameof(ItemRestoreHistoriesReadModel.Id),
                Desc: true);

            var result = validator.Validate(criteria);

            result.IsValid.Should().BeTrue();

            paginationValidatorMock.Verify(
                v => v.Validate(It.Is<ValidationContext<Pagination>>(
                    ctx => ctx.InstanceToValidate.Equals(pagination))),
                Times.Once);

            filterValidatorMock.Verify(
                v => v.Validate(It.Is<ValidationContext<ItemRestoreHistoriesFilter>>(
                    ctx => ctx.InstanceToValidate == filter)),
                Times.Once);
        }

        [Fact]
        public void Validate_Should_Fail_When_Pagination_Is_Null()
        {
            var validator = CreateValidator(out var paginationValidatorMock, out _);

            var criteria = new ItemRestoreHistoriesPageCriteria(
                Pagination: null!,
                Filter: null,
                SortBy: nameof(ItemRestoreHistoriesReadModel.Id),
                Desc: true);

            var result = validator.Validate(criteria);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(ItemRestoreHistoriesPageCriteria.Pagination));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(ItemRestoreHistoriesPageCriteria.Pagination));

            paginationValidatorMock.Verify(
                v => v.Validate(It.IsAny<ValidationContext<Pagination>>()),
                Times.Never);
        }

        public static TheoryData<string> AllowedSortColumns() =>
        [
            nameof(ItemRestoreHistoriesReadModel.Id),
            nameof(ItemRestoreHistoriesReadModel.AsOfUtc),
            nameof(ItemRestoreHistoriesReadModel.CurrentUser),
            nameof(ItemRestoreHistoriesReadModel.StartedAtUtc),
            nameof(ItemRestoreHistoriesReadModel.EndedAtUtc),
            nameof(ItemRestoreHistoriesReadModel.AffectedCounts),
        ];

        [Theory]
        [MemberData(nameof(AllowedSortColumns))]
        public void Validate_Should_Pass_When_SortBy_Is_Allowed(string sortBy)
        {
            var validator = CreateValidator(out _, out _);

            var criteria = new ItemRestoreHistoriesPageCriteria(
                Pagination: new Pagination(),
                Filter: null,
                SortBy: sortBy,
                Desc: true);

            var result = validator.Validate(criteria);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_SortBy_Is_Empty()
        {
            var validator = CreateValidator(out _, out _);

            var criteria = new ItemRestoreHistoriesPageCriteria(
                Pagination: new Pagination(),
                Filter: null,
                SortBy: string.Empty,
                Desc: true);

            var result = validator.Validate(criteria);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(ItemRestoreHistoriesPageCriteria.SortBy));
        }

        [Fact]
        public void Validate_Should_Fail_When_SortBy_Is_Not_Allowed()
        {
            var validator = CreateValidator(out _, out _);

            var criteria = new ItemRestoreHistoriesPageCriteria(
                Pagination: new Pagination(),
                Filter: null,
                SortBy: "UnknownColumn",
                Desc: true);

            var result = validator.Validate(criteria);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(ItemRestoreHistoriesPageCriteria.SortBy));
        }
    }
}
