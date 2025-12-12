using FluentValidation;
using GameTools.Server.Application.Common.Paging;

namespace GameTools.Server.Application.Features.Restores.Queries.GetItemRestoreHistoriesPage
{
    public sealed class ItemRestoreHistoriesPageCriteriaValidator : AbstractValidator<ItemRestoreHistoriesPageCriteria>
    {
        private static readonly HashSet<string> AllowedSortColumns =
        [
            nameof(ItemRestoreHistoriesReadModel.Id),
            nameof(ItemRestoreHistoriesReadModel.AsOfUtc),
            nameof(ItemRestoreHistoriesReadModel.CurrentUser),
            nameof(ItemRestoreHistoriesReadModel.StartedAtUtc),
            nameof(ItemRestoreHistoriesReadModel.EndedAtUtc),
            nameof(ItemRestoreHistoriesReadModel.AffectedCounts)
        ];

        public ItemRestoreHistoriesPageCriteriaValidator(
            IValidator<Pagination> paginationValidator,
            IValidator<ItemRestoreHistoriesFilter> restoreHistoriesFilterValidator)
        {
            RuleFor(x => x.Pagination)
                .NotNull()
                .SetValidator(paginationValidator);

            When(x => x.Filter is not null, () =>
            {
                RuleFor(x => x.Filter!)
                    .SetValidator(restoreHistoriesFilterValidator);
            });

            RuleFor(x => x.SortBy)
                .NotEmpty()
                .Must(s => AllowedSortColumns.Contains(s!))
                .WithMessage($"Sort By must be one of: {string.Join(", ", AllowedSortColumns)}");
        }
    }
}
