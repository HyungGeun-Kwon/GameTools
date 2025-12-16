using FluentValidation;

namespace GameTools.Server.Application.Operations.Restores.Queries.GetItemRestoreHistoriesPage
{
    public sealed class ItemRestoreHistoriesFilterValidator : AbstractValidator<ItemRestoreHistoriesFilter>
    {
        public ItemRestoreHistoriesFilterValidator()
        {
            When(x => x.FromUtc.HasValue && x.ToUtc.HasValue, () =>
            {
                RuleFor(x => x).Must(x => x.FromUtc <= x.ToUtc)
                    .WithMessage("FromUtc must be <= ToUtc.");
            });
        }
    }
}
