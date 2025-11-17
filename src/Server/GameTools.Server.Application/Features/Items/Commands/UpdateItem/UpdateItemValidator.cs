using FluentValidation;
using GameTools.Server.Domain.Common.Rules;

namespace GameTools.Server.Application.Features.Items.Commands.UpdateItem
{
    public sealed class UpdateItemValidator : AbstractValidator<UpdateItemCommand>
    {
        public UpdateItemValidator()
        {
            RuleFor(x => x.Payload).NotEmpty();
            RuleFor(x => x.Payload.Id).GreaterThan(0);
            RuleFor(x => x.Payload.Name).NotEmpty().MaximumLength(ItemRules.NameMax);
            RuleFor(x => x.Payload.Price).GreaterThanOrEqualTo(ItemRules.PriceMin);
            RuleFor(x => x.Payload.Description).MaximumLength(ItemRules.DescriptionMax);
            RuleFor(x => x.Payload.RarityId).GreaterThan((byte)0);
            RuleFor(x => x.Payload.RowVersion).NotEmpty();
        }
    }
}
