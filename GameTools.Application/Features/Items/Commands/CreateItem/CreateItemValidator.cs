using FluentValidation;
using GameTools.Domain.Common.Rules;

namespace GameTools.Application.Features.Items.Commands.CreateItem
{
    public sealed class CreateItemValidator : AbstractValidator<CreateItemCommand>
    {
        public CreateItemValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(ItemRules.NameMax);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(ItemRules.PriceMin);
            RuleFor(x => x.Description).MaximumLength(ItemRules.DescriptionMax);
            RuleFor(x => x.RarityId).GreaterThan((byte)0);
        }
    }
}
