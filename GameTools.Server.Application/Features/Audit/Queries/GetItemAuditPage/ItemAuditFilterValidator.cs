using FluentValidation;

namespace GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage
{
    public sealed class ItemAuditFilterValidator : AbstractValidator<ItemAuditFilter>
    {
        public ItemAuditFilterValidator()
        {
            When(x => !string.IsNullOrWhiteSpace(x.Action), () =>
            {
                RuleFor(x => x.Action!)
                    .Must(a => a is "INSERT" or "UPDATE" or "DELETE")
                    .WithMessage("Action must be one of INSERT/UPDATE/DELETE.");
            });

            When(x => x.FromUtc.HasValue && x.ToUtc.HasValue, () =>
            {
                RuleFor(x => x).Must(x => x.FromUtc <= x.ToUtc)
                    .WithMessage("FromUtc must be <= ToUtc.");
            });
        }
    }
}
