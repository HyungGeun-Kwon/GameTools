using FluentValidation;

namespace GameTools.Server.Application.Operations.Restores.Commands.RestoreItems
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

            When(x => x.ItemIds is not null, () =>
            {
                RuleFor(x => x.ItemIds!)
                    .Must(ids => ids.Count > 0)
                    .WithMessage("ItemIds cannot be empty when provided.");

                RuleForEach(x => x.ItemIds).NotEmpty();
            });
        }
    }
}
