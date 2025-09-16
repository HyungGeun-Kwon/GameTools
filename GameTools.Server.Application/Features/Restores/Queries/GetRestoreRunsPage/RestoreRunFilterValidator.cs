using FluentValidation;

namespace GameTools.Server.Application.Features.Restores.Queries.GetRestoreRunsPage
{
    public sealed class RestoreRunFilterValidator : AbstractValidator<RestoreRunFilter>
    {
        public RestoreRunFilterValidator()
        {
            When(x => x.FromUtc.HasValue && x.ToUtc.HasValue, () =>
            {
                RuleFor(x => x).Must(x => x.FromUtc <= x.ToUtc)
                    .WithMessage("FromUtc must be <= ToUtc.");
            });
        }
    }
}
