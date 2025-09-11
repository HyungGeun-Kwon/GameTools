using FluentValidation;
using GameTools.Server.Application.Common.Paging;

namespace GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage
{
    public sealed class GetItemAuditPageValidator : AbstractValidator<GetItemAuditPageQuery>
    {
        public GetItemAuditPageValidator()
        {
            RuleFor(x => x.Criteria.Pagination).NotNull();

            RuleFor(x => x.Criteria.Pagination)
                .SetValidator(new PaginationValidator());

            When(x => x.Criteria.Filter is not null, () =>
            {
                RuleFor(x => x.Criteria.Filter!)
                    .SetValidator(new ItemAuditFilterValidator());
            });
        }
    }
}
