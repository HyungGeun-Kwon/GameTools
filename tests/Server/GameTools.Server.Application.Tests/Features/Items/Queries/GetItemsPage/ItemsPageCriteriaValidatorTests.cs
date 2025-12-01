using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Items.Models;
using GameTools.Server.Application.Features.Items.Queries.GetItemsPage;
using Moq;
using static GameTools.Server.Application.Tests.Features.Items.Queries.GetItemsPage.GetItemsPageTestData;

namespace GameTools.Server.Application.Tests.Features.Items.Queries.GetItemsPage
{
    public class ItemsPageCriteriaValidatorTests
    {
        private static ItemsPageCriteriaValidator CreateValidator(
            out Mock<IValidator<Pagination>> paginationValidatorMock,
            out Mock<IValidator<ItemsFilter>> filterValidatorMock)
        {
            paginationValidatorMock = new Mock<IValidator<Pagination>>();
            paginationValidatorMock
                .Setup(v => v.Validate(It.IsAny<Pagination>()))
                .Returns(new ValidationResult());

            filterValidatorMock = new Mock<IValidator<ItemsFilter>>();
            filterValidatorMock
                .Setup(v => v.Validate(It.IsAny<ItemsFilter>()))
                .Returns(new ValidationResult());

            return new ItemsPageCriteriaValidator(
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
            var filter = new ItemsFilter();

            var criteria = BuildCriteria(pagination, filter);

            var result = validator.Validate(criteria);

            result.IsValid.Should().BeTrue();

            paginationValidatorMock.Verify(
                v => v.Validate(It.IsAny<ValidationContext<Pagination>>()),
                Times.Once);

            filterValidatorMock.Verify(
                v => v.Validate(It.IsAny<ValidationContext<ItemsFilter>>()),
                Times.Once);
        }

        [Fact]
        public void Validate_Should_Fail_When_Pagination_Is_Null()
        {
            var validator = CreateValidator(
                out var paginationValidatorMock,
                out _);

            var criteria = new ItemsPageCriteria(
                Pagination: null!,
                Filter: null,
                SortBy: nameof(ItemReadModel.Id),
                Desc: true);

            var result = validator.Validate(criteria);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(ItemsPageCriteria.Pagination));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(ItemsPageCriteria.Pagination));

            paginationValidatorMock.Verify(
                v => v.Validate(It.IsAny<ValidationContext<Pagination>>()),
                Times.Never);
        }

        public static TheoryData<string> AllowedSortColumns() =>
        [
            nameof(ItemReadModel.Id),
            nameof(ItemReadModel.Name),
            nameof(ItemReadModel.Price)
        ];

        [Theory]
        [MemberData(nameof(AllowedSortColumns))]
        public void Validate_Should_Pass_When_SortBy_Is_Allowed(string sortBy)
        {
            var validator = CreateValidator(
                out _,
                out _);

            var criteria = BuildCriteria(sortBy: sortBy);

            var result = validator.Validate(criteria);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_SortBy_Is_Empty()
        {
            var validator = CreateValidator(
                out _,
                out _);

            var criteria = BuildCriteria(sortBy: string.Empty);

            var result = validator.Validate(criteria);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(ItemsPageCriteria.SortBy));
        }

        [Fact]
        public void Validate_Should_Fail_When_SortBy_Is_Not_Allowed()
        {
            var validator = CreateValidator(
                out _,
                out _);

            var criteria = BuildCriteria(sortBy: "UnknownColumn");

            var result = validator.Validate(criteria);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(ItemsPageCriteria.SortBy));
        }
    }
}
