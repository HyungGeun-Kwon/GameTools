using FluentValidation;
using GameTools.Server.Application.Common.Paging;

namespace GameTools.Server.Application.Features.Items.Queries.GetItemPage
{
    public sealed class GetItemsPageValidator : AbstractValidator<GetItemsPageQuery>
    {
        public GetItemsPageValidator()
        {
            RuleFor(x => x.Criteria.Pagination).NotNull();

            RuleFor(x => x.Criteria.Pagination)
                .SetValidator(new PaginationValidator());

            When(x => x.Criteria.Filter is not null, () =>
            {
                RuleFor(x => x.Criteria.Filter!)
                    .SetValidator(new ItemFilterValidator());
            });
        }
    }
}
