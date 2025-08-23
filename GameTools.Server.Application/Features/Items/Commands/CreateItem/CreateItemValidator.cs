using FluentValidation;
using GameTools.Server.Domain.Common.Rules;

namespace GameTools.Server.Application.Features.Items.Commands.CreateItem
{
    public sealed class CreateItemValidator : AbstractValidator<CreateItemCommand>
    {
        public CreateItemValidator()
        {
            RuleFor(x => x.Payload).NotEmpty();
            RuleFor(x => x.Payload.Name).NotEmpty().MaximumLength(ItemRules.NameMax);
            RuleFor(x => x.Payload.Price).GreaterThanOrEqualTo(ItemRules.PriceMin);
            RuleFor(x => x.Payload.Description).MaximumLength(ItemRules.DescriptionMax);
            RuleFor(x => x.Payload.RarityId).GreaterThan((byte)0);
        }
    }
}
