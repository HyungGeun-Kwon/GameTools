using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GameTools.Server.Application.Features.Items.Queries.GetItemsPage;
using Moq;
using static GameTools.Server.Application.Tests.Features.Items.Queries.GetItemsPage.GetItemsPageTestData;

namespace GameTools.Server.Application.Tests.Features.Items.Queries.GetItemsPage
{
    public class GetItemsPageValidatorTests
    {
        private static GetItemsPageValidator CreateValidator(
            out Mock<IValidator<ItemsPageCriteria>> criteriaValidatorMock)
        {
            criteriaValidatorMock = new Mock<IValidator<ItemsPageCriteria>>();
            criteriaValidatorMock
                .Setup(v => v.Validate(It.IsAny<ValidationContext<ItemsPageCriteria>>()))
                .Returns(new ValidationResult());

            return new GetItemsPageValidator(criteriaValidatorMock.Object);
        }

        private static GetItemsPageQuery BuildValidQuery()
        {
            var criteria = BuildCriteria();
            return new GetItemsPageQuery(criteria);
        }

        [Fact]
        public void Validate_Should_Pass_When_Query_Is_Valid()
        {
            var query = BuildValidQuery();
            var validator = CreateValidator(out var criteriaValidatorMock);

            var result = validator.Validate(query);

            result.IsValid.Should().BeTrue();
            criteriaValidatorMock.Verify(
                v => v.Validate(It.Is<ValidationContext<ItemsPageCriteria>>(
                    ctx => ctx.InstanceToValidate == query.Criteria)),
                Times.Once);
        }

        [Fact]
        public void Validate_Should_Fail_When_Criteria_Is_Null()
        {
            var validator = CreateValidator(out var criteriaValidatorMock);
            var query = new GetItemsPageQuery(null!);

            var result = validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(GetItemsPageQuery.Criteria));
            result.Errors.Should().OnlyContain(e => e.PropertyName == nameof(GetItemsPageQuery.Criteria));

            criteriaValidatorMock.Verify(
                v => v.Validate(It.IsAny<ValidationContext<ItemsPageCriteria>>()),
                Times.Never);
        }
    }
}
