using FluentValidation;
using GameTools.Application.Common.Paging;

namespace GameTools.Application.Features.Items.Queries.GetItemPage
{
    public sealed class GetItemsPageValidator : AbstractValidator<GetItemsPageQuery>
    {
        public GetItemsPageValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, PagingRules.MaxPageSize);
            RuleFor(x => x.RarityId).GreaterThan((byte)0).When(x => x.RarityId.HasValue);
        }
    }
}
