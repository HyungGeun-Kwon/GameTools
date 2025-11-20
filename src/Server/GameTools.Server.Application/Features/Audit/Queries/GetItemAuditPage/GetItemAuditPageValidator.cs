using FluentValidation;

namespace GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage
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
