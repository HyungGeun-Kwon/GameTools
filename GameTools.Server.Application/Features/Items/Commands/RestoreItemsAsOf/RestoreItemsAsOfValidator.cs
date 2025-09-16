using FluentValidation;

namespace GameTools.Server.Application.Features.Items.Commands.RestoreItemsAsOf
{
    public sealed class RestoreItemsAsOfValidator : AbstractValidator<RestoreItemsAsOfCommand>
    {
        public RestoreItemsAsOfValidator()
        {
            RuleFor(x => x.Payload).NotEmpty();
            
            RuleFor(x => x.Payload.AsOfUtc)
                .NotEmpty().WithMessage("AsOfUtc is required.")
                .LessThanOrEqualTo(_ => DateTime.UtcNow.AddMinutes(1))
                .WithMessage("AsOfUtc must be in the past (UTC).");

            RuleFor(x => x.Payload.ItemId)
                .GreaterThan(0).When(x => x.Payload.ItemId.HasValue)
                .WithMessage("ItemId must be > 0 when provided.");

            RuleFor(x => x.Payload.Notes)
                .MaximumLength(4000);
        }
    }
}
