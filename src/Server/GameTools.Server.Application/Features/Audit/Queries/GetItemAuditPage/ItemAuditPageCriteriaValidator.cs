using FluentValidation;
using GameTools.Server.Application.Common.Paging;

namespace GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage
{
    public sealed class ItemAuditPageCriteriaValidator : AbstractValidator<ItemAuditPageCriteria>
    {
        private static readonly HashSet<string> AllowedSortColumns =
        [
            nameof(ItemAuditReadModel.Id),
            nameof(ItemAuditReadModel.ItemId),
            nameof(ItemAuditReadModel.ChangedAtUtc),
        ];
        public ItemAuditPageCriteriaValidator(
            IValidator<Pagination> paginationValidator,
            IValidator<ItemAuditFilter> itemAuditFilterValidator)
        {
            RuleFor(x => x.Pagination)
                .NotNull()
                .SetValidator(paginationValidator);

            When(x => x.Filter is not null, () =>
            {
                RuleFor(x => x.Filter!)
                    .SetValidator(itemAuditFilterValidator);
            });

            RuleFor(x => x.SortBy)
                .NotEmpty()
                .Must(s => AllowedSortColumns.Contains(s!))
                .WithMessage($"Sort By must be one of: {string.Join(", ", AllowedSortColumns)}");
        }
    }
}
