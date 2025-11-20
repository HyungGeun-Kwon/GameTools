using FluentValidation;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using GameTools.Server.Domain.Features.Items.ValueObjects;

namespace GameTools.Server.Application.Features.Items.Commands.Common.Validations
{
    public sealed class CreateItemSpecValidator : AbstractValidator<CreateItemSpec>
    {
        public CreateItemSpecValidator()
        {
            RuleFor(d => d.Name).NotEmpty()
                .MinimumLength(ItemName.MinLength)
                .MaximumLength(ItemName.MaxLength);
            RuleFor(d => d.Price).GreaterThanOrEqualTo(ItemPrice.MinValue);
            RuleFor(d => d.Description).MaximumLength(ItemDescription.MaxLength);
            RuleFor(d => d.RarityId).NotEmpty();
        }
    }
}
