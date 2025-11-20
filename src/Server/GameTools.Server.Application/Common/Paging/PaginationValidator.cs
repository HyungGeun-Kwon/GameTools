using FluentValidation;
using GameTools.Server.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace GameTools.Server.Application.Common.Paging
{
    public sealed class PaginationValidator : AbstractValidator<Pagination>
    {
        public PaginationValidator(IOptions<PagingOptions> pagingOptions)
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, pagingOptions.Value.MaxPageSize);
        }
    }
}
