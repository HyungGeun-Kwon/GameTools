using FluentValidation;

namespace GameTools.Server.Application.Features.Restores.Commands.RestoreItems
{
    public class RestoreItemsSpecValidator : AbstractValidator<RestoreItemsSpec>
    {
        public RestoreItemsSpecValidator()
        {
            RuleFor(x => x.AsOfUtc)
                .NotEmpty()
                .WithMessage("AsOfUtc is required.")
                .LessThanOrEqualTo(_ => DateTime.UtcNow.AddMinutes(1))
                .WithMessage("AsOfUtc must be in the past (UTC).");

            When(x => x.ItemId.HasValue, () =>
            {
                RuleFor(x => x.ItemId).GreaterThan(0).WithMessage("ItemId must be > 0 when provided.");
            });
        }
    }
}
