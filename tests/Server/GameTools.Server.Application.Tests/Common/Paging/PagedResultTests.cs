using FluentAssertions;
using GameTools.Server.Application.Common.Paging;

namespace GameTools.Server.Application.Tests.Common.Paging
{
    public class PagedResultTests
    {
        [Fact]
        public void TotalPages_Should_Be_Computed_Correctly()
        {
            var items = new[] { 1, 2, 3, 4, 5 };
            var paged = new PagedResult<int>(items, 23, 2, 10);

            paged.TotalPages.Should().Be(3);
            paged.HasPrevious.Should().BeTrue();
            paged.HasNext.Should().BeTrue();
        }

        [Fact]
        public void TotalPages_Should_Be_Zero_When_PageSize_Is_Zero()
        {
            var items = Array.Empty<int>();
            var paged = new PagedResult<int>(items, 10, 1, 0);

            paged.TotalPages.Should().Be(0);
            paged.HasPrevious.Should().BeFalse();
            paged.HasNext.Should().BeFalse();
        }

        [Fact]
        public void HasPrevious_And_HasNext_Should_Be_Correct_On_Edge_Pages()
        {
            var items = new[] { 1, 2, 3 };

            var firstPage = new PagedResult<int>(items, 30, 1, 10);
            firstPage.HasPrevious.Should().BeFalse();
            firstPage.HasNext.Should().BeTrue();

            var lastPage = new PagedResult<int>(items, 30, 3, 10);
            lastPage.HasPrevious.Should().BeTrue();
            lastPage.HasNext.Should().BeFalse();
        }
    }
}
