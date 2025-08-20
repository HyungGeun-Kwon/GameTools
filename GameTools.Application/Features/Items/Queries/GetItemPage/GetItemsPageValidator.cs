using FluentValidation;
using GameTools.Application.Common.Paging;

namespace GameTools.Application.Features.Items.Queries.GetItemPage
{
    public sealed class GetItemsPageValidator : AbstractValidator<GetItemsPageQuery>
    {
        public GetItemsPageValidator()
        {
            RuleFor(x => x.GetItemsPageQueryParams.Pagination).NotNull();

            RuleFor(x => x.GetItemsPageQueryParams.Pagination)
                .SetValidator(new PaginationValidator());

            When(x => x.GetItemsPageQueryParams.Filter is not null, () =>
            {
                RuleFor(x => x.GetItemsPageQueryParams.Filter!)
                    .SetValidator(new ItemFilterValidator());
            });
        }
    }
}
