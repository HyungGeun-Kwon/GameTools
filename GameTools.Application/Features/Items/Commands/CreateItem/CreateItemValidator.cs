using FluentValidation;
using GameTools.Domain.Common.Rules;

namespace GameTools.Application.Features.Items.Commands.CreateItem
{
    public sealed class CreateItemValidator : AbstractValidator<CreateItemCommand>
    {
        public CreateItemValidator()
        {
            RuleFor(x => x.ItemCreateDto).NotEmpty();
            RuleFor(x => x.ItemCreateDto.Name).NotEmpty().MaximumLength(ItemRules.NameMax);
            RuleFor(x => x.ItemCreateDto.Price).GreaterThanOrEqualTo(ItemRules.PriceMin);
            RuleFor(x => x.ItemCreateDto.Description).MaximumLength(ItemRules.DescriptionMax);
            RuleFor(x => x.ItemCreateDto.RarityId).GreaterThan((byte)0);
        }
    }
}
