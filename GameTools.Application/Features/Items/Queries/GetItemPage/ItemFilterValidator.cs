using FluentValidation;

namespace GameTools.Application.Features.Items.Queries.GetItemPage
{
    public sealed class ItemFilterValidator : AbstractValidator<ItemFilter>
    {
        public ItemFilterValidator()
        {
            RuleFor(x => x.Search).MaximumLength(100);
            RuleFor(x => x.RarityId)
                .GreaterThan((byte)0)
                .When(x => x.RarityId.HasValue);
        }
    }
}
