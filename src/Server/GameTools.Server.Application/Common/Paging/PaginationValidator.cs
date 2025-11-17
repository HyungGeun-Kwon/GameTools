using FluentValidation;

namespace GameTools.Server.Application.Common.Paging
{
    public sealed class PaginationValidator : AbstractValidator<Pagination>
    {
        public PaginationValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, PagingRules.MaxPageSize);
        }
    }
}
