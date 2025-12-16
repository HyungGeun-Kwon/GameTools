using FluentValidation;

namespace GameTools.Server.Application.Operations.Restores.Queries.GetItemRestoreHistoriesPage
{
    public sealed class GetItemRestoreHistoriesPageValidator : AbstractValidator<GetItemRestoreHistoriesPageQuery>
    {
        public GetItemRestoreHistoriesPageValidator(IValidator<ItemRestoreHistoriesPageCriteria> criteriaValidator)
        {
            RuleFor(x => x.Criteria)
                .NotNull()
                .SetValidator(criteriaValidator);
        }
    }
}
