using FluentValidation;

namespace GameTools.Server.Application.Auditing.Queries.GetItemAuditPage
{
    public sealed class ItemAuditFilterValidator : AbstractValidator<ItemAuditFilter>
    {
        private static readonly HashSet<string> AllowedActionColumns = ["INSERT", "UPDATE", "DELETE"];

        public ItemAuditFilterValidator()
        {
            When(x => x.Actions != null && x.Actions.Any(), () =>
            {
                RuleForEach(x => x.Actions)
                    .Must(a => AllowedActionColumns.Contains(a))
                    .WithMessage($"Action must be one of: {string.Join(", ", AllowedActionColumns)}");
            });

            When(x => x.FromUtc.HasValue && x.ToUtc.HasValue, () =>
            {
                RuleFor(x => x).Must(x => x.FromUtc <= x.ToUtc)
                    .WithMessage("FromUtc must be <= ToUtc.");
            });
        }
    }
}
