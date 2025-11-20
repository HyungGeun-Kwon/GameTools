using FluentValidation;
using GameTools.Server.Application.Common.Paging;
using GameTools.Server.Application.Features.Items.Models;

namespace GameTools.Server.Application.Features.Items.Queries.GetItemsPage
{
    public sealed class ItemsPageCriteriaValidator : AbstractValidator<ItemsPageCriteria>
    {
        private static readonly HashSet<string> AllowedSortColumns =
        [
            nameof(ItemReadModel.Id),
            nameof(ItemReadModel.Name),
            nameof(ItemReadModel.Price)
        ];

        public ItemsPageCriteriaValidator(
            IValidator<Pagination> paginationValidator,
            IValidator<ItemsFilter> itemFilterValidator)
        {
            RuleFor(x => x.Pagination)
                .NotNull()
                .SetValidator(paginationValidator);

            When(x => x.Filter is not null, () =>
            {
                RuleFor(x => x.Filter!)
                    .SetValidator(itemFilterValidator);
            });

            RuleFor(x => x.SortBy)
                .NotEmpty()
                .Must(s => AllowedSortColumns.Contains(s!))
                .WithMessage($"SortBy must be one of: {string.Join(", ", AllowedSortColumns)}");
        }
    }
}
