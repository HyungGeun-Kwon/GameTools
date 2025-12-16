using FluentValidation;

namespace GameTools.Server.Application.Catalog.Items.Queries.GetItemsPage
{
    public sealed class GetItemsPageValidator : AbstractValidator<GetItemsPageQuery>
    {
        public GetItemsPageValidator(IValidator<ItemsPageCriteria> criteriaValidator)
        {
            RuleFor(x => x.Criteria)
                .NotNull()
                .SetValidator(criteriaValidator);
        }
    }
}
