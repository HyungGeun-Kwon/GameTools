using FluentValidation;
using GameTools.Server.Domain.Catalog.Items.ValueObjects;

namespace GameTools.Server.Application.Catalog.Items.Queries.GetItemsPage
{
    public sealed class ItemsFilterValidator : AbstractValidator<ItemsFilter>
    {
        public ItemsFilterValidator()
        {
            RuleFor(x => x.Search).MaximumLength(ItemName.MaxLength);

            When(x => x.RarityIds is not null, () =>
            {
                RuleFor(x => x.RarityIds!)
                    .NotEmpty()
                    .Must(list => list.Distinct().Count() == list.Count)
                    .WithMessage("Duplicate rarity ids are not allowed.");
            });
        }
    }
}
