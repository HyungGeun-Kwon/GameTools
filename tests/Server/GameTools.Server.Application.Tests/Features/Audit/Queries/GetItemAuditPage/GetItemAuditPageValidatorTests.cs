using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage;
using Moq;

namespace GameTools.Server.Application.Tests.Features.Audit.Queries.GetItemAuditPage
{
    public class GetItemAuditPageValidatorTests
    {
        private static GetItemAuditPageValidator CreateValidator(
            out Mock<IValidator<ItemAuditPageCriteria>> criteriaValidatorMock)
        {
            criteriaValidatorMock = new Mock<IValidator<ItemAuditPageCriteria>>();
            criteriaValidatorMock
                .Setup(v => v.Validate(It.IsAny<ValidationContext<ItemAuditPageCriteria>>()))
                .Returns(new ValidationResult());

            return new GetItemAuditPageValidator(criteriaValidatorMock.Object);
        }

        private static GetItemAuditPageQuery BuildValidQuery()
        {
            var criteria = new ItemAuditPageCriteria(
                Pagination: new GameTools.Server.Application.Common.Paging.Pagination(),
                Filter: null,
                SortBy: nameof(ItemAuditReadModel.Id),
                Desc: true);

            return new GetItemAuditPageQuery(criteria);
        }

        [Fact]
        public void Validate_Should_Pass_When_Query_Is_Valid()
        {
            var validator = CreateValidator(out var criteriaValidatorMock);
            var query = BuildValidQuery();

            var result = validator.Validate(query);

            result.IsValid.Should().BeTrue();

            criteriaValidatorMock.Verify(
                v => v.Validate(It.Is<ValidationContext<ItemAuditPageCriteria>>(
                    ctx => ctx.InstanceToValidate == query.Criteria)),
                Times.Once);
        }

        [Fact]
        public void Validate_Should_Fail_When_Criteria_Is_Null()
        {
            var validator = CreateValidator(out var criteriaValidatorMock);
            var query = new GetItemAuditPageQuery(null!);

            var result = validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(GetItemAuditPageQuery.Criteria));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(GetItemAuditPageQuery.Criteria));

            criteriaValidatorMock.Verify(
                v => v.Validate(It.IsAny<ValidationContext<ItemAuditPageCriteria>>()),
                Times.Never);
        }
    }
}
