using FluentValidation;
using GameTools.Domain.Common.Rules;

namespace GameTools.Application.Features.Items.Commands.UpdateItem
{
    public sealed class UpdateItemValidator : AbstractValidator<UpdateItemCommand>
    {
        public UpdateItemValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(ItemRules.NameMax);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(ItemRules.PriceMin);
            RuleFor(x => x.Description).MaximumLength(ItemRules.DescriptionMax);
            RuleFor(x => x.RarityId).GreaterThan((byte)0);
        }
    }
}
