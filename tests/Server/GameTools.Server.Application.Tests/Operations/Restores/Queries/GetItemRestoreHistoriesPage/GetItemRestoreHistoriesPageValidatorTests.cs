using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GameTools.Server.Application.Operations.Restores.Queries.GetItemRestoreHistoriesPage;
using Moq;

namespace GameTools.Server.Application.Tests.Operations.Restores.Queries.GetItemRestoreHistoriesPage
{
    public class GetItemRestoreHistoriesPageValidatorTests
    {
        private static GetItemRestoreHistoriesPageValidator CreateValidator(
            out Mock<IValidator<ItemRestoreHistoriesPageCriteria>> criteriaValidatorMock)
        {
            criteriaValidatorMock = new Mock<IValidator<ItemRestoreHistoriesPageCriteria>>();
            criteriaValidatorMock
                .Setup(v => v.Validate(It.IsAny<ValidationContext<ItemRestoreHistoriesPageCriteria>>()))
                .Returns(new ValidationResult());

            return new GetItemRestoreHistoriesPageValidator(criteriaValidatorMock.Object);
        }

        private static GetItemRestoreHistoriesPageQuery BuildValidQuery()
        {
            var criteria = new ItemRestoreHistoriesPageCriteria(
                Pagination: new GameTools.Server.Application.Common.Paging.Pagination(),
                Filter: null,
                SortBy: nameof(ItemRestoreHistoriesReadModel.Id),
                Desc: true);

            return new GetItemRestoreHistoriesPageQuery(criteria);
        }

        [Fact]
        public void Validate_Should_Pass_When_Query_Is_Valid()
        {
            var validator = CreateValidator(out var criteriaValidatorMock);
            var query = BuildValidQuery();

            var result = validator.Validate(query);

            result.IsValid.Should().BeTrue();

            criteriaValidatorMock.Verify(
                v => v.Validate(It.Is<ValidationContext<ItemRestoreHistoriesPageCriteria>>(
                    ctx => ctx.InstanceToValidate == query.Criteria)),
                Times.Once);
        }

        [Fact]
        public void Validate_Should_Fail_When_Criteria_Is_Null()
        {
            var validator = CreateValidator(out var criteriaValidatorMock);
            var query = new GetItemRestoreHistoriesPageQuery(null!);

            var result = validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(GetItemRestoreHistoriesPageQuery.Criteria));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(GetItemRestoreHistoriesPageQuery.Criteria));

            criteriaValidatorMock.Verify(
                v => v.Validate(It.IsAny<ValidationContext<ItemRestoreHistoriesPageCriteria>>()),
                Times.Never);
        }
    }
}
