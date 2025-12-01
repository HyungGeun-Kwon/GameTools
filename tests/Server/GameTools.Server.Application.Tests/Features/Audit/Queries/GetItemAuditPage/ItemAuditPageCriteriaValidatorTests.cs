using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage;
using Moq;

namespace GameTools.Server.Application.Tests.Features.Audit.Queries.GetItemAuditPage
{
    public class ItemAuditPageCriteriaValidatorTests
    {
        private static ItemAuditPageCriteriaValidator CreateValidator(
            out Mock<IValidator<Pagination>> paginationValidatorMock,
            out Mock<IValidator<ItemAuditFilter>> filterValidatorMock)
        {
            paginationValidatorMock = new Mock<IValidator<Pagination>>();
            paginationValidatorMock
                .Setup(v => v.Validate(It.IsAny<ValidationContext<Pagination>>()))
                .Returns(new ValidationResult());

            filterValidatorMock = new Mock<IValidator<ItemAuditFilter>>();
            filterValidatorMock
                .Setup(v => v.Validate(It.IsAny<ValidationContext<ItemAuditFilter>>()))
                .Returns(new ValidationResult());

            return new ItemAuditPageCriteriaValidator(
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
            var filter = new ItemAuditFilter(
                ItemId: Guid.NewGuid(),
                Actions: ["INSERT"],
                FromUtc: null,
                ToUtc: null);

            var criteria = new ItemAuditPageCriteria(
                Pagination: pagination,
                Filter: filter,
                SortBy: nameof(ItemAuditReadModel.Id),
                Desc: true);

            var result = validator.Validate(criteria);

            result.IsValid.Should().BeTrue();

            paginationValidatorMock.Verify(
                v => v.Validate(It.Is<ValidationContext<Pagination>>(
                    ctx => ctx.InstanceToValidate.Equals(pagination))),
                Times.Once);

            filterValidatorMock.Verify(
                v => v.Validate(It.Is<ValidationContext<ItemAuditFilter>>(
                    ctx => ctx.InstanceToValidate == filter)),
                Times.Once);
        }

        [Fact]
        public void Validate_Should_Fail_When_Pagination_Is_Null()
        {
            var validator = CreateValidator(
                out var paginationValidatorMock,
                out _);

            var criteria = new ItemAuditPageCriteria(
                Pagination: null!,
                Filter: null,
                SortBy: nameof(ItemAuditReadModel.Id),
                Desc: true);

            var result = validator.Validate(criteria);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(ItemAuditPageCriteria.Pagination));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(ItemAuditPageCriteria.Pagination));

            paginationValidatorMock.Verify(v => v.Validate(It.IsAny<ValidationContext<Pagination>>()), Times.Never);
        }

        public static TheoryData<string> AllowedSortColumns() =>
        [
            nameof(ItemAuditReadModel.Id),
            nameof(ItemAuditReadModel.ItemId),
            nameof(ItemAuditReadModel.ChangedAtUtc),
        ];

        [Theory]
        [MemberData(nameof(AllowedSortColumns))]
        public void Validate_Should_Pass_When_SortBy_Is_Allowed(string sortBy)
        {
            var validator = CreateValidator(out _, out _);

            var pagination = new Pagination();

            var criteria = new ItemAuditPageCriteria(
                Pagination: pagination,
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

            var pagination = new Pagination();

            var criteria = new ItemAuditPageCriteria(
                Pagination: pagination,
                Filter: null,
                SortBy: string.Empty,
                Desc: true);

            var result = validator.Validate(criteria);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(ItemAuditPageCriteria.SortBy));
        }

        [Fact]
        public void Validate_Should_Fail_When_SortBy_Is_Not_Allowed()
        {
            var validator = CreateValidator(out _, out _);

            var pagination = new Pagination();

            var criteria = new ItemAuditPageCriteria(
                Pagination: pagination,
                Filter: null,
                SortBy: "UnknownColumn",
                Desc: true);

            var result = validator.Validate(criteria);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(ItemAuditPageCriteria.SortBy));
        }
    }
}
