using FluentValidation;

namespace GameTools.Server.Application.Auditing.Queries.GetItemAuditPage
{
    public sealed class GetItemAuditPageValidator : AbstractValidator<GetItemAuditPageQuery>
    {
        public GetItemAuditPageValidator(IValidator<ItemAuditPageCriteria> criteriaValidator)
        {
            RuleFor(x => x.Criteria)
                .NotNull()
                .SetValidator(criteriaValidator);
        }
    }
}
