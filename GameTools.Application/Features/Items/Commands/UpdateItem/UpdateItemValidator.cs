using FluentValidation;
using GameTools.Domain.Common.Rules;

namespace GameTools.Application.Features.Items.Commands.UpdateItem
{
    public sealed class UpdateItemValidator : AbstractValidator<UpdateItemCommand>
    {
        public UpdateItemValidator()
        {
            RuleFor(x => x.ItemUpdateDto).NotEmpty();
            RuleFor(x => x.ItemUpdateDto.Id).GreaterThan(0);
            RuleFor(x => x.ItemUpdateDto.Name).NotEmpty().MaximumLength(ItemRules.NameMax);
            RuleFor(x => x.ItemUpdateDto.Price).GreaterThanOrEqualTo(ItemRules.PriceMin);
            RuleFor(x => x.ItemUpdateDto.Description).MaximumLength(ItemRules.DescriptionMax);
            RuleFor(x => x.ItemUpdateDto.RarityId).GreaterThan((byte)0);
        }
    }
}
