using FluentAssertions;
using GameTools.Server.Application.Common.Options;
using GameTools.Server.Application.Common.Paging;
using Microsoft.Extensions.Options;

namespace GameTools.Server.Application.Tests.Common.Paging
{
    public class PaginationValidatorTests
    {
        private static PaginationValidator CreateValidator(int maxPageSize = 100)
        {
            var options = Options.Create(new PagingOptions
            {
                DefaultPageSize = 20,
                MaxPageSize = maxPageSize
            });

            return new PaginationValidator(options);
        }

        [Fact]
        public void Validate_Should_Pass_When_PageNumber_And_PageSize_InRange()
        {
            var validator = CreateValidator();
            var pagination = new Pagination(PageNumber: 1, PageSize: 50);

            var result = validator.Validate(pagination);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_Should_Fail_When_PageNumber_LessThan1()
        {
            var validator = CreateValidator();
            var pagination = new Pagination(PageNumber: 0, PageSize: 20);

            var result = validator.Validate(pagination);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(Pagination.PageNumber));
        }

        [Fact]
        public void Validate_Should_Fail_When_PageSize_ExceedsMax()
        {
            var validator = CreateValidator();
            var pagination = new Pagination(PageNumber: 1, PageSize: 101);

            var result = validator.Validate(pagination);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(Pagination.PageSize));
        }
    }
}
