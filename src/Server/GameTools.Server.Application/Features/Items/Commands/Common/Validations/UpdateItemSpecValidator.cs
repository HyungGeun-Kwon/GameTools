using FluentValidation;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using GameTools.Server.Domain.Features.Items.ValueObjects;

namespace GameTools.Server.Application.Features.Items.Commands.Common.Validations
{
    public sealed class UpdateItemSpecValidator : AbstractValidator<UpdateItemSpec>
    {
        public UpdateItemSpecValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(ItemName.MinLength)
                .MaximumLength(ItemName.MaxLength);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(ItemPrice.MinValue);
            RuleFor(x => x.Description).MaximumLength(ItemDescription.MaxLength);
            RuleFor(x => x.RowVersion).NotEmpty().NotNull();
        }
    }
}
